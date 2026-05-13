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
    private const int GroupCount = 12;
    private const int BomboSize = 12;
    private const int MatchesPerGroup = 6;
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;

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
    
    public Fixture GenerateGroupsStageFixture(TournamentConfig config)
    {
        var teams = _teamRepository.GetAll();
        var stadiums = _stadiumRepository.GetAllStadiums();
        if (teams.Count != 48) throw new Exception("Se necesitan 48 equipos");
        
        if (stadiums.Count < 4) throw new Exception("Se necesitan 4 estadios");
        
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        
        var orderedTeams = OrderTeamsWithTieBreak(teams, config.SemillaFixture, config);
        DistributeTeamsToGroups(orderedTeams, fixture.GetGroups.ToList());
        
        GenerateMatches(fixture, stadiums, config);
        
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        _fixtureRepository.AddFixture(fixture);
        return fixture;
    }

    private List<Team> OrderTeamsWithTieBreak(List<Team> teams)
    {
        var rng = new Random(_config.SemillaFixture);
        var result = new List<Team>();

        var groupedByRanking = teams.GroupBy(t => t.FifaRanking)
            .OrderByDescending(g => g.Key);

        foreach (var group in groupedByRanking)
        {
            var teamsInGroup = group.ToList();
            if (teamsInGroup.Count > 1)
                teamsInGroup = ApplyFisherYates(teamsInGroup, rng);
            result.AddRange(teamsInGroup);
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
        foreach (var team in bomboTeams)
        {
            bool assigned = false;
            int startIndex = bomboTeams.IndexOf(team) % groups.Count;
            for (int offset = 0; offset < groups.Count && !assigned; offset++)
            {
                int groupIndex = (startIndex + offset) % groups.Count;
                var group = groups[groupIndex];
            
                if (group.TeamsInGroup.Count >= 4)
                    continue;
            
                if (CanAddTeamToGroup(team, group))
                {
                    group.AddTeamToGroup(team);
                    assigned = true;
                }
            }
            if (!assigned)
                throw new InvalidOperationException($"No se pudo ubicar al equipo {team.Name} en ningún grupo.");
        }
    }
    
    private bool CanAddTeamToGroup(Team team, Group group)
    {
        var confederation = team.Confederation.Name;
        var teamsInGroup = group.TeamsInGroup;
        if (confederation == "UEFA")
        {
            int uefaCount = teamsInGroup.Count(t => ((Team)t).Confederation.Name == "UEFA");
            return uefaCount < 2;
        }
        bool confederationAlreadyPresent = teamsInGroup.Any(t => ((Team)t).Confederation.Name == confederation);
        return !confederationAlreadyPresent;
    }

    private void GenerateMatches(Fixture fixture, List<Stadium> stadiums, TournamentConfig config)
    {
        var orderedStadiums = stadiums.OrderBy(s => s.StadiumName).ToList();
        var clock = new ClockStub(DateTime.Now);
        int matchCounter = 0;
        
        foreach (var group in fixture.GetGroups)
        {
            var teams = group.TeamsInGroup.ToList();
            var matchups = new (int home, int away)[]
            {
                (0, 3), (1, 2), (0, 2), (1, 3), (0, 1), (2, 3) 
            };
            
            for (int i = 0; i < matchups.Length; i++)
            {
                int jornada = i / 2;
                DateTime fecha = config.FechaInicioTorneo.AddDays(jornada * config.SeparacionEntreFechas);
                if (jornada == 2) fecha = fecha.Date.AddHours(14);
                else
                    fecha = (i % 2 == 0) ? fecha.Date.AddHours(14) : fecha.Date.AddHours(18);
                
                var estadio = orderedStadiums[matchCounter % orderedStadiums.Count];
                var match = new Match(
                    fecha,
                    estadio,
                    teams[matchups[i].home],
                    teams[matchups[i].away],
                    group.GroupLetter,
                    clock
                );
                group.AddMatchToGroup(match);
                matchCounter++;
            }
        }
    }
}