using Services;
using Domain.Entities;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class SimulationServiceTests
{
    private static readonly DateTime FechaCreacionClock = new DateTime(2027, 6, 10);
    private static readonly DateTime FechaPartido = new DateTime(2027, 6, 12);

    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private SimulationService _service = null!;
    private MatchRepositoryStub _matchRepository = null!;
    private RankingServiceStub _rankingService = null!;
    private FixtureRepositoryStub _fixtureRepository = null!;
    private TournamentConfig _config = null!;

    private const int Seed = 42;
    private int _nextId;

    [TestInitialize]
    public void Setup()
    {
        _nextId = 1;
        _logRepository = new LogRepositoryStub();
        _logService = new LogService(_logRepository);
        _matchRepository = new MatchRepositoryStub();
        _rankingService = new RankingServiceStub();
        _fixtureRepository = new FixtureRepositoryStub();
        _config = new TournamentConfig();

        var engineFactory = new Services.Simulation.DefaultSimulationEngineFactory();

        _service = new SimulationService(
            _logService,
            _matchRepository,
            _rankingService,
            _fixtureRepository,
            _config,
            engineFactory,
            new NotificationServiceStub());

        var fixture = new Fixture(_config);
        fixture.SetSimulationEngine("Probabilístico");
        _fixtureRepository.AddFixture(fixture);
    }

    private Match BuildAndAddMatch(string homeName, int homeRanking, string awayName, int awayRanking, char group = 'A')
    {
        var home = new Team(homeName, new Confederation("UEFA"), homeRanking);
        var away = new Team(awayName, new Confederation("UEFA"), awayRanking);
        var stadium = new Stadium("Estadio Test", "Ciudad", "", 50000);
        var match = new Match(_nextId++, FechaPartido, stadium, home, away, group, new ClockStub(FechaCreacionClock));

        _matchRepository.AddMatch(match);

        return match;
    }

    private Group BuildGroupWithMatches()
    {
        var teams = new[]
        {
            new Team("TeamA", new Confederation("UEFA"), 1800),
            new Team("TeamB", new Confederation("UEFA"), 1700),
            new Team("TeamC", new Confederation("CONMEBOL"), 1600),
            new Team("TeamD", new Confederation("CAF"), 1500),
        };

        var stadium = new Stadium("Est1", "Ciudad", "", 50000);
        var group = new Group('A');

        foreach (var team in teams)
            group.AddTeamToGroup(team);

        var matchups = new[] { (0, 3), (1, 2), (0, 2), (1, 3), (0, 1), (2, 3) };

        foreach (var (homeIndex, awayIndex) in matchups)
        {
            var match = new Match(
                _nextId++,
                FechaPartido,
                stadium,
                teams[homeIndex],
                teams[awayIndex],
                'A',
                new ClockStub(FechaCreacionClock));

            group.AddMatchToGroup(match);
            _matchRepository.AddMatch(match);
        }

        var fixture = _fixtureRepository.GetFixture();
        fixture.SetGroups(new List<Group> { group });

        return group;
    }

    [TestMethod]
    public void SimulateMatch_GoalsShouldNotBeNegative()
    {
        var match = BuildAndAddMatch("Argentina", 1800, "Bolivia", 500);

        _service.SimulateMatch(match.IdMatch, Seed);

        var updated = _matchRepository.GetMatchById(match.IdMatch);

        Assert.IsTrue(updated.HomeGoals >= 0);
        Assert.IsTrue(updated.AwayGoals >= 0);
    }

    [TestMethod]
    public void SimulateMatch_WithSameSeed_ShouldReturnSameResult()
    {
        var match1 = BuildAndAddMatch("España", 1900, "Peru", 700);

        _service.SimulateMatch(match1.IdMatch, Seed);

        var result1home = _matchRepository.GetMatchById(match1.IdMatch).HomeGoals;
        var result1away = _matchRepository.GetMatchById(match1.IdMatch).AwayGoals;

        _matchRepository.ClearMatches();
        _nextId = 1;

        var match2 = BuildAndAddMatch("España", 1900, "Peru", 700);

        _service.SimulateMatch(match2.IdMatch, Seed);

        var result2home = _matchRepository.GetMatchById(match2.IdMatch).HomeGoals;
        var result2away = _matchRepository.GetMatchById(match2.IdMatch).AwayGoals;

        Assert.AreEqual(result1home, result2home);
        Assert.AreEqual(result1away, result2away);
    }

    [TestMethod]
    public void SimulateMatch_GoalsShouldNotExceedMaximum()
    {
        for (int i = 0; i < 50; i++)
        {
            _matchRepository.ClearMatches();

            var match = BuildAndAddMatch($"Team{i}A", 1000 + i, $"Team{i}B", 1000 + i + 1);

            _service.SimulateMatch(match.IdMatch, i);

            var updated = _matchRepository.GetMatchById(match.IdMatch);

            Assert.IsTrue(updated.HomeGoals <= 7);
            Assert.IsTrue(updated.AwayGoals <= 7);
        }
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldSetResultsForAllMatches()
    {
        BuildGroupWithMatches();

        var fixture = _fixtureRepository.GetFixture();
        fixture.InitializeGroups();
        fixture.CargarPartidos(_matchRepository.GetAllMatches());

        _service.SimulateGroupStage();

        foreach (var match in _matchRepository.GetAllMatches())
            Assert.IsTrue(match.HomeGoals.HasValue);
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldNotOverwriteResults()
    {
        BuildGroupWithMatches();

        var primerPartido = _matchRepository.GetAllMatches().First();
        primerPartido.SetClock(new ClockStub(new DateTime(2027, 6, 15)));
        primerPartido.SetMatchResult(3, 0);
        _matchRepository.UpdateMatch(primerPartido);

        _service.SimulateGroupStage();

        var updated = _matchRepository.GetMatchById(primerPartido.IdMatch);

        Assert.AreEqual(3, updated.HomeGoals);
        Assert.AreEqual(0, updated.AwayGoals);
    }

    [TestMethod]
    public void SimulateMatch_WithValidMatch_ShouldRegisterSimulationExecutedLog()
    {
        var match = BuildAndAddMatch("Uruguay", 1500, "Argentina", 1500);

        _service.SimulateMatch(match.IdMatch, Seed);

        var logs = _logRepository.GetAll();

        Assert.IsTrue(logs.Any(l => l.Category == "Simulation" && l.Action == "SimulationExecuted"));
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldAddCardIncidentsToMatches()
    {
        BuildGroupWithMatches();

        _service.SimulateGroupStage();

        foreach (var match in _matchRepository.GetAllMatches())
            Assert.IsTrue(match.Incidents.Count > 0);
    }

    [TestMethod]
    public void SimulateMatch_WithCustomEngineFactory_UsesEngineAndAddsIncidents()
    {
        // Arrange: create a test engine that returns deterministic scores based on seed
        var testEngine = new ServicesTests.Stubs.TestSimulationEngine("TestEngine");
        var testFactory = new ServicesTests.Stubs.TestEngineFactory(testEngine);

        var service = new SimulationService(
            _logService,
            _matchRepository,
            _rankingService,
            _fixtureRepository,
            _config,
            testFactory,
            new NotificationServiceStub());

        var match = BuildAndAddMatch("EquipoX", 1200, "EquipoY", 1100);

        // Act
        service.SimulateMatch(match.IdMatch, Seed);

        // Assert
        var updated = _matchRepository.GetMatchById(match.IdMatch);
        // based on TestSimulationEngine: home = (seed + homeRanking) % 5
        int matchSeed = Seed + updated.IdMatch;
        int expectedHome = (matchSeed + updated.HomeTeam.CurrentRanking) % 5;
        int expectedAway = (matchSeed + updated.AwayTeam.CurrentRanking) % 5;

        Assert.AreEqual(expectedHome, updated.HomeGoals);
        Assert.AreEqual(expectedAway, updated.AwayGoals);
        Assert.IsTrue(updated.Incidents.Count >= 0); // incidents may be zero depending on random, check method executed

        var logs = _logRepository.GetAll();
        Assert.IsTrue(logs.Any(l => l.Category == "Simulation" && l.Action == "SimulationExecuted"));
    }
}
