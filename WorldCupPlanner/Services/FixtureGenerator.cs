using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
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
    private const string ConfederationUefa = "UEFA";
    private const int MaxUefaTeamsPerGroup = 2;
    
    private int _nextMatchId = 1;

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
        _nextMatchId = 1;
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
        DistributeTeamsToGroups(orderedTeams, fixture.Groups.ToList());
        GenerateMatches(fixture, stadiums);

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        _fixtureRepository.AddFixture(fixture);
        return fixture;
    }

    private List<Team> OrderTeamsWithTieBreak(List<Team> teams)
    {
        var randomGenerator = new Random(_config.SemillaFixture);
        var result = new List<Team>();

        var groupedByRanking = teams
            .GroupBy(t => t.FifaRanking)
            .OrderByDescending(g => g.Key);

        foreach (var rankingGroup in groupedByRanking)
        {
            var tied = rankingGroup.ToList();
            var shuffledTeams = (tied.Count > 1)
                ? ApplyFisherYates(tied, randomGenerator)
                : tied;
            result.AddRange(shuffledTeams);
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
        bool success = TryAssignAll(orderedTeams, 0, groups);
        if (!success)
            throw new InvalidOperationException("No se pudo distribuir los equipos respetando las restricciones de confederación.");
    }

    private bool TryAssignAll(List<Team> teams, int index, List<Group> groups)
    {
        if (index == teams.Count) return true;

        var team = teams[index];
        int roundRobinGroup = index % groups.Count;

        for (int offset = 0; offset < groups.Count; offset++)
        {
            int groupIndex = (roundRobinGroup + offset) % groups.Count;
            var group = groups[groupIndex];

            if (group.TeamsInGroup.Count >= TeamsPerGroup) continue;
            if (!CanAddTeamToGroup(team, group)) continue;

            group.AddTeamToGroup(team);
            if (TryAssignAll(teams, index + 1, groups)) return true;
            group.RemoveTeamFromGroup(team);
        }
        return false;
    }

    private static bool CanAddTeamToGroup(Team team, Group group)
    {
        var teams = group.TeamsInGroup;
        var confederation = team.Confederation.Name;

        if (confederation == ConfederationUefa)
            return teams.Count(t => t.Confederation.Name == ConfederationUefa) < MaxUefaTeamsPerGroup;

        return teams.All(t => t.Confederation.Name != confederation);
    }

    private void GenerateMatches(Fixture fixture, List<Stadium> stadiums)
    {
        var orderedStadiums = stadiums
            .OrderBy(s => NormalizeName(s.StadiumName))
            .ToList();

        var groups = fixture.Groups.ToList();
        int matchCounter = 0;

        foreach (var (groupIndex, group) in groups.Select((g, i) => (i, g)))
        {
            matchCounter = GenerateGroupMatches(group, orderedStadiums, groupIndex, matchCounter);
        }
    }

    private int GenerateGroupMatches(Group group, List<Stadium> orderedStadiums, int groupIndex, int matchCounter)
    {
        var teams     = group.TeamsInGroup.ToList();
        var matchDates = CalculateMatchDatesForGroup(groupIndex);
        var matchups  = GetGroupRoundRobinMatchups();

        for (int i = 0; i < matchups.Length; i++)
        {
            var stadium = orderedStadiums[matchCounter % orderedStadiums.Count];
            var match   = new Match(
                _nextMatchId++,
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
        return matchCounter;
    }

    private static (int home, int away)[] GetGroupRoundRobinMatchups()
    {
        // Round-robin: todos contra todos (4 equipos = 6 partidos totales)
        return new (int home, int away)[]
        {
            (0, 3), (1, 2),  // Jornada 1
            (0, 2), (1, 3),  // Jornada 2
            (0, 1), (2, 3),  // Jornada 3
        };
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
        var lower = name.ToLowerInvariant();
        var normalized = string.Concat(lower.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));
        return string.Join(" ", normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

}