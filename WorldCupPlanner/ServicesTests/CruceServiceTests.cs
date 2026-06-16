using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DomainTests.Stubs;
using IRepository;
using Services;
using Services.Simulation;
using ServicesTests.Stubs;
using ISimulationEngineFactory = IServices.ISimulationEngineFactory;

namespace ServicesTests;

[TestClass]
public class CruceServiceTests
{
    private CruceService _service = null!;
    private CrucesRepositoryStub _crucesRepository = null!;
    private StadiumRepositoryStub _stadiumRepository = null!;
    private IMatchRepository _matchRepository = null!;
    private FixtureRepositoryStub _fixtureRepository = null!;
    private TeamRepositoryStub _teamRepository = null!;
    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private TournamentConfig _config = null!;
    private DateTimeMatchStub _clock = null!;
    private ISimulationEngineFactory _engineFactory = null!;

    [TestInitialize]
    public void Setup()
    {
        Fixture.RegisterEngine("Aleatorio puro", () => new PureRandomSimulationEngine());
        Fixture.RegisterEngine("Probabilístico", () => new ProbabilisticSimulationEngine());

        _crucesRepository = new CrucesRepositoryStub();
        _stadiumRepository = new StadiumRepositoryStub();
        _matchRepository = new MatchRepositoryStub();
        _fixtureRepository = new FixtureRepositoryStub();
        _teamRepository = new TeamRepositoryStub();
        _logRepository = new LogRepositoryStub();
        _logService = new LogService(_logRepository);
        _config = new TournamentConfig();
        _clock = new DateTimeMatchStub(new DateTime(2026, 1, 1));
        _engineFactory = new DefaultSimulationEngineFactory(new List<ISimulationEngine>
        {
            new PureRandomSimulationEngine(),
            new ProbabilisticSimulationEngine()
        });

        _service = new CruceService(
            _crucesRepository,
            _stadiumRepository,
            _config,
            _matchRepository,
            _fixtureRepository,
            _clock,
            _engineFactory,
            _teamRepository,
            _logService);
    }

    private Team CreateTeam(string name, string confederation = "CONMEBOL", int ranking = 1000)
    {
        var team = new Team(name, new Confederation(confederation), ranking);
        _teamRepository.Add(team);
        return team;
    }

    private Stadium CreateStadium(string name = "Estadio1")
    {
        var stadium = new Stadium(name, "Ciudad", null, 50000);
        _stadiumRepository.AddStadium(stadium);
        return stadium;
    }

    private Fixture CreateFixtureWithEngine()
    {
        var fixture = new Fixture(_config);
        fixture.SetSimulationEngine("Probabilístico");
        _fixtureRepository.AddFixture(fixture);
        return fixture;
    }

    private Match CreateMatchWithResult(Team home, Team away, Stadium stadium, int homeGoals, int awayGoals)
    {
        var match = new Match(
            _matchRepository.GetNextId(),
            new DateTime(2027, 6, 10),
            stadium,
            home,
            away,
            'N',
            _clock);

        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.AddMatch(match);

        return match;
    }

    private Fixture CreateFixtureReadyForRoundOf32()
    {
        var fixture = new Fixture(_config);
        fixture.InitializeGroups();

        foreach (var group in Create12GroupsWithFinishedMatches())
        {
            var fixtureGroup = fixture.Groups.First(g => g.GroupLetter == group.GroupLetter);

            foreach (var team in group.TeamsInGroup)
                fixtureGroup.AddTeamToGroup(team);

            foreach (var match in group.MatchesInGroup)
            {
                fixtureGroup.AddMatchToGroup(match);
                _matchRepository.AddMatch(match);
            }
        }

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        _fixtureRepository.AddFixture(fixture);

        return fixture;
    }

    private List<Group> Create12GroupsWithFinishedMatches()
    {
        var groups = new List<Group>();

        for (char letter = 'A'; letter <= 'L'; letter++)
        {
            var group = new Group(letter);

            var team1 = CreateTeam($"Team{letter}1", "CONMEBOL", 1800);
            var team2 = CreateTeam($"Team{letter}2", "UEFA", 1700);
            var team3 = CreateTeam($"Team{letter}3", "CAF", 1600);
            var team4 = CreateTeam($"Team{letter}4", "AFC", 1500);

            group.AddTeamToGroup(team1);
            group.AddTeamToGroup(team2);
            group.AddTeamToGroup(team3);
            group.AddTeamToGroup(team4);

            AddFinishedGroupMatch(group, team1, team2, 3, 0);
            AddFinishedGroupMatch(group, team1, team3, 2, 0);
            AddFinishedGroupMatch(group, team1, team4, 1, 0);
            AddFinishedGroupMatch(group, team2, team3, 2, 1);
            AddFinishedGroupMatch(group, team2, team4, 1, 0);
            AddFinishedGroupMatch(group, team3, team4, 1, 0);

            groups.Add(group);
        }

        return groups;
    }

    private void AddFinishedGroupMatch(Group group, Team home, Team away, int homeGoals, int awayGoals)
    {
        var stadium = CreateStadium($"Estadio_{group.GroupLetter}_{_matchRepository.GetNextId()}");

        var match = new Match(
            _matchRepository.GetNextId(),
            new DateTime(2027, 6, 10),
            stadium,
            home,
            away,
            group.GroupLetter,
            _clock);

        match.SimulateResult(homeGoals, awayGoals);
        group.AddMatchToGroup(match);
    }

    private List<Cruce> CreateFinishedCruces(int count, TournamentPhase phase, string prefix)
    {
        var cruces = new List<Cruce>();

        for (int i = 1; i <= count; i++)
        {
            var home = CreateTeam($"{prefix}Home{i}", "CONMEBOL", 1500 + i);
            var away = CreateTeam($"{prefix}Away{i}", "UEFA", 1400 + i);
            var stadium = CreateStadium($"{prefix}Stadium{i}");

            var match = CreateMatchWithResult(home, away, stadium, 2, 1);

            cruces.Add(new Cruce($"{prefix}{i}", match, phase));
        }

        return cruces;
    }

    [TestMethod]
    public void GetAllCruces_WhenNoCruces_ReturnsEmptyList()
    {
        var result = _service.GetAllCruces();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_WhenNoFixture_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateRoundOf32());
    }

    [TestMethod]
    public void GenerateNextRound_WhenNoCruces_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void GenerateNextRound_WhenCrucesHaveNoResult_ThrowsInvalidOperationException()
    {
        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");
        var stadium = CreateStadium();

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);

        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf32);
        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void SimulateCruces_WhenNoFixture_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.SimulateCruces());
    }

    [TestMethod]
    public void SimulateCruces_WithPendingCruces_SetsResultOnAllMatches()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);

        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf32);
        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        _service.SimulateCruces();

        Assert.IsTrue(match.HomeGoals.HasValue);
        Assert.IsTrue(match.AwayGoals.HasValue);
    }

    [TestMethod]
    public void SimulateCruces_NeverProducesDrawResult()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);

        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        Assert.AreNotEqual(match.HomeGoals, match.AwayGoals);
    }

    [TestMethod]
    public void SimulateCruces_UpdatesTeamRankingsAfterMatch()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay", "CONMEBOL", 1500);
        var away = CreateTeam("Argentina", "UEFA", 1200);

        int rankingHomeAntes = home.CurrentRanking;
        int rankingAwayAntes = away.CurrentRanking;

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);
        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        var homeActualizado = _teamRepository.Get("Uruguay")!;
        var awayActualizado = _teamRepository.Get("Argentina")!;

        Assert.AreNotEqual(rankingHomeAntes, homeActualizado.CurrentRanking);
        Assert.AreNotEqual(rankingAwayAntes, awayActualizado.CurrentRanking);
    }

    [TestMethod]
    public void SimulateCruces_LogsRankingUpdate()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);
        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        var logs = _logRepository.GetAll();

        Assert.IsTrue(logs.Any(l => l.Category == "Ranking"));
    }

    [TestMethod]
    public void SimulateCruces_SkipsCrucesWithExistingResult()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = CreateMatchWithResult(home, away, stadium, 2, 1);
        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }

    [TestMethod]
    public void GenerateRoundOf32_WithCompletedGroupStage_ShouldGenerateCruces()
    {
        CreateFixtureReadyForRoundOf32();

        for (int i = 1; i <= 4; i++)
            CreateStadium($"EstadioExtra{i}");

        _service.GenerateRoundOf32();

        var cruces = _crucesRepository.GetAllCruces();

        Assert.AreEqual(16, cruces.Count);
        Assert.IsTrue(cruces.All(c => c.Phase == TournamentPhase.RoundOf32));
    }

    [TestMethod]
    public void GenerateRoundOf32_WhenGroupsAreNotCompleted_ShouldThrowInvalidOperationException()
    {
        var fixture = new Fixture(_config);
        fixture.InitializeGroups();
        _fixtureRepository.AddFixture(fixture);

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateRoundOf32());
    }

    [TestMethod]
    public void GenerateNextRound_WhenCurrentPhaseIsFinal_ShouldThrowInvalidOperationException()
    {
        var home = CreateTeam("FinalHome");
        var away = CreateTeam("FinalAway", "UEFA");
        var stadium = CreateStadium("FinalStadium");

        var match = CreateMatchWithResult(home, away, stadium, 2, 1);

        _crucesRepository.SetCruces(new List<Cruce>
        {
            new Cruce("F1", match, TournamentPhase.Final)
        });

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void GenerateNextRound_FromRoundOf32_ShouldGenerateRoundOf16()
    {
        for (int i = 1; i <= 4; i++)
            CreateStadium($"Round16Stadium{i}");

        var cruces = new List<Cruce>();
        cruces.AddRange(CreateFinishedCruces(8, TournamentPhase.RoundOf32, "A"));
        cruces.AddRange(CreateFinishedCruces(8, TournamentPhase.RoundOf32, "B"));

        _crucesRepository.SetCruces(cruces);

        _service.GenerateNextRound();

        var result = _crucesRepository.GetAllCruces();

        Assert.IsTrue(result.Any(c => c.Phase == TournamentPhase.RoundOf16));
    }

    [TestMethod]
    public void GenerateNextRound_FromQuarterFinals_ShouldGenerateSemiFinals()
    {
        for (int i = 1; i <= 4; i++)
            CreateStadium($"SemiStadium{i}");

        var cruces = CreateFinishedCruces(4, TournamentPhase.QuarterFinals, "D");
        _crucesRepository.SetCruces(cruces);

        _service.GenerateNextRound();

        var result = _crucesRepository.GetAllCruces();

        Assert.IsTrue(result.Any(c => c.Phase == TournamentPhase.SemiFinals));
    }

    [TestMethod]
    public void GenerateNextRound_FromSemiFinals_ShouldGenerateFinalAndThirdPlace()
    {
        for (int i = 1; i <= 4; i++)
            CreateStadium($"FinalStadium{i}");

        var cruces = CreateFinishedCruces(2, TournamentPhase.SemiFinals, "S");
        _crucesRepository.SetCruces(cruces);

        _service.GenerateNextRound();

        var result = _crucesRepository.GetAllCruces();

        Assert.IsTrue(result.Any(c => c.Phase == TournamentPhase.Final));
        Assert.IsTrue(result.Any(c => c.Phase == TournamentPhase.ThirdPlace));
    }

    [TestMethod]
    public void GetAllCruces_WhenCrucesExist_ReturnsMappedCruces()
    {
        var home = CreateTeam("MappedHome");
        var away = CreateTeam("MappedAway", "UEFA");
        var stadium = CreateStadium("MappedStadium");

        var match = CreateMatchWithResult(home, away, stadium, 2, 1);
        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf32);

        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        var result = _service.GetAllCruces();

        Assert.AreEqual(1, result.Count);
    }
}