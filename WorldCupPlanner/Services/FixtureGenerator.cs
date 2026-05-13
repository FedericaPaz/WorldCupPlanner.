using Domain;
using IRepository;

namespace Services;

public class FixtureGenerator
{
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly TournamentConfig _config;
    private readonly IDateTimeMatch _clock;

    private const int TeamsPerGroup = 4;
    private const int BomboSize = 12;
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;
    private const int GroupsPerBatch = 6;

    public FixtureGenerator(
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IFixtureRepository fixtureRepository,
        TournamentConfig config,
        IDateTimeMatch clock)
    {
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _fixtureRepository = fixtureRepository;
        _config = config;
        _clock = clock;
    }

    public Fixture GenerateGroupsStageFixture()
    {
        var teams = _teamRepository.GetAll();
        var stadiums = _stadiumRepository.GetAllStadiums();

        if (teams.Count != 48)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");
        if (stadiums.Count < 4)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

        var fixture = new Fixture(_config);
        fixture.InitializeGroups();
        var orderedTeams = OrderTeamsWithTieBreak(teams);
        DistributeTeamsToGroups(orderedTeams, fixture.GetGroups.ToList());
        GenerateMatches(fixture, stadiums);
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        _fixtureRepository.AddFixture(fixture);
        return fixture;
    }
    
    private List<Team> OrderTeamsWithTieBreak(List<Team> teams)
    {
        var rng = new Random(_config.SemillaFixture);
        var result = new List<Team>();

        var groupedByRanking = teams
            .GroupBy(t => t.FifaRanking)
            .OrderByDescending(g => g.Key);
        foreach (var rankingGroup in groupedByRanking)
        {
            var tied = rankingGroup.ToList();
            if (tied.Count > 1)
                tied = ApplyFisherYates(tied, rng);
            result.AddRange(tied);
        }
        return result;
    }

    private static List<T> ApplyFisherYates<T>(List<T> list, Random rng)
    {
        var arr = list.ToArray();
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr.ToList();
    }


    private void DistributeTeamsToGroups(List<Team> orderedTeams, List<Group> groups)
    {
        for (int bombo = 0; bombo < 4; bombo++)
        {
            var bomboTeams = orderedTeams.Skip(bombo * BomboSize).Take(BomboSize).ToList();
            DistributeBomboWithConfederationRules(bomboTeams, groups);
        }
    }

    private void DistributeBomboWithConfederationRules(List<Team> bomboTeams, List<Group> groups)
    {
        for (int i = 0; i < bomboTeams.Count; i++)
        {
            var team = bomboTeams[i];
            bool assigned = false;

            for (int offset = 0; offset < groups.Count && !assigned; offset++)
            {
                int groupIndex = (i + offset) % groups.Count;
                var group = groups[groupIndex];

                if (group.TeamsInGroup.Count >= TeamsPerGroup) continue;
                if (CanAddTeamToGroup(team, group))
                {
                    group.AddTeamToGroup(team);
                    assigned = true;
                }
            }
            if (!assigned) throw new InvalidOperationException($"No se pudo ubicar al equipo {team.Name} en ningún grupo respetando las restricciones de confederación.");
        }
    }

    private static bool CanAddTeamToGroup(Team team, Group group)
    {
        var teams = group.TeamsInGroup;
        var confederation = team.Confederation.Name;
        if (confederation == "UEFA")
        {
            int uefaCount = teams.Count(t => t.Confederation.Name == "UEFA");
            return uefaCount < 2;
        }
        return teams.All(t => t.Confederation.Name != confederation);
    }


    private void GenerateMatches(Fixture fixture, List<Stadium> stadiums)
    {
        var orderedStadiums = stadiums
            .OrderBy(s => NormalizeName(s.StadiumName))
            .ToList();

        var groups = fixture.GetGroups.ToList();
        int matchCounter = 0;
        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            var group = groups[groupIndex];
            var teams = group.TeamsInGroup.ToList();
            var matchDates = CalculateMatchDatesForGroup(groupIndex);
            var matchups = new (int home, int away)[]
            {
                (0, 3), (1, 2),  
                (0, 2), (1, 3),  
                (0, 1), (2, 3), 
            };

            for (int i = 0; i < matchups.Length; i++)
            {
                var stadium = orderedStadiums[matchCounter % orderedStadiums.Count];
                var match = new Match(
                    matchDates[i],
                    stadium,
                    teams[matchups[i].home],
                    teams[matchups[i].away],
                    group.GroupLetter,
                    _clock
                );
                group.AddMatchToGroup(match);
                matchCounter++;
            }
        }
    }
    
    private DateTime[] CalculateMatchDatesForGroup(int groupIndex)
    {
        int batchIndex = groupIndex / GroupsPerBatch;
        int positionInBatch = groupIndex % GroupsPerBatch;

        DateTime round1Date = CalculateRoundDate(batchIndex, roundOffset: 0, positionInBatch);
        DateTime round2Date = CalculateRoundDate(batchIndex, roundOffset: _config.SeparacionEntreFechas, positionInBatch);
        DateTime round3Date = CalculateRoundDate(batchIndex, roundOffset: _config.SeparacionEntreFechas * 2, positionInBatch);

        return new[]
        {
            round1Date, round1Date,  
            round2Date, round2Date,  
            round3Date, round3Date, 
        };
    }
    
    private DateTime CalculateRoundDate(int batchIndex, int roundOffset, int positionInBatch)
    {
        int daysFromStart = batchIndex + roundOffset;
        int hour = StartHour + (positionInBatch % _config.MaxPartidosPorDia) * HoursBetweenMatches;
        return _config.FechaInicioTorneo.Date.AddDays(daysFromStart).AddHours(hour);
    }
    
    private static string NormalizeName(string name)
    {
        return string.Join(' ', name.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
