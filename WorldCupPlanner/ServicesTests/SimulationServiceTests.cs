using Services;
using Domain.Entities;
using Domain.Enums;
using IRepository;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class SimulationServiceTests
{
    private static readonly DateTime FechaCreacionClock = new DateTime(2027, 6, 10);
    private static readonly DateTime FechaPartido = new DateTime(2027, 6, 12);
    private static readonly DateTime FechaResultadoClock = new DateTime(2027, 6, 15);

    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private SimulationService _service = null!;
    private IMatchRepository _matchRepository = null!;
    private ProbabilisticSimulationEngine _engine = null!;
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

        _service = new SimulationService(
            _logService,
            _matchRepository,
            _rankingService,
            _fixtureRepository,
            _config);

        _engine = new ProbabilisticSimulationEngine();
    }

    private Match BuildMatch(string homeName, int homeRanking, string awayName, int awayRanking, char group = 'A')
    {
        var home = new Team(homeName, new Confederation("UEFA"), homeRanking);
        var away = new Team(awayName, new Confederation("UEFA"), awayRanking);
        var stadium = new Stadium("Estadio Test", "Ciudad", "", 50000);

        return new Match(
            _nextId++, FechaPartido, stadium, home, away, group,
            new ClockStub(FechaCreacionClock));
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
                _nextId++, FechaPartido, stadium,
                teams[homeIndex], teams[awayIndex], 'A',
                new ClockStub(FechaCreacionClock));
            group.AddMatchToGroup(match);
        }

        return group;
    }

    private Cruce BuildCruce(string nombre)
    {
        var home = new Team("Alemania", new Confederation("UEFA"), 2000);
        var away = new Team("Australia", new Confederation("AFC"), 400);
        var stadium = new Stadium("Test Stadium2", "Ciudad", "", 50000);

        var match = new Match(
            _nextId++, FechaPartido, stadium, home, away, 'N',
            new ClockStub(FechaCreacionClock));

        return new Cruce(nombre, match, TournamentPhase.RoundOf32);
    }

    [TestMethod]
    public void SimulateMatch_GoalsShouldNotBeNegative()
    {
        var match = BuildMatch("Argentina", 1800, "Bolivia", 500);
        var (home, away) = _service.SimulateMatch(match, Seed, _engine);
        Assert.IsTrue(home >= 0);
        Assert.IsTrue(away >= 0);
    }

    [TestMethod]
    public void SimulateMatch_WithSameSeed_ShouldReturnSameResult()
    {
        var stadium = new Stadium("Estadio Test", "Ciudad", "", 50000);
        var clock = new ClockStub(FechaCreacionClock);

        var match1 = new Match(1, FechaPartido, stadium,
            new Team("España", new Confederation("UEFA"), 1900),
            new Team("Peru", new Confederation("UEFA"), 700), 'A', clock);

        var match2 = new Match(1, FechaPartido, stadium,
            new Team("España", new Confederation("UEFA"), 1900),
            new Team("Peru", new Confederation("UEFA"), 700), 'A', clock);

        var result1 = _service.SimulateMatch(match1, Seed, _engine);
        var result2 = _service.SimulateMatch(match2, Seed, _engine);

        Assert.AreEqual(result1.homeGoals, result2.homeGoals);
        Assert.AreEqual(result1.awayGoals, result2.awayGoals);
    }

    [TestMethod]
    public void SimulateMatch_DifferentSeeds_ShouldReturnDifferentResults()
    {
        var match1 = BuildMatch("Francia", 2000, "Ecuador", 600);
        var match2 = BuildMatch("Francia", 2000, "Ecuador", 600);

        var result1 = _service.SimulateMatch(match1, 1, _engine);
        var result2 = _service.SimulateMatch(match2, 99999, _engine);

        bool sonDistintos = result1.homeGoals != result2.homeGoals || result1.awayGoals != result2.awayGoals;
        Assert.IsTrue(sonDistintos);
    }

    [TestMethod]
    public void SimulateMatch_GoalsShouldNotExceedMaximum()
    {
        for (int i = 0; i < 50; i++)
        {
            var match = BuildMatch($"Team{i}A", 1000 + i, $"Team{i}B", 1000 + i + 1);
            var (home, away) = _service.SimulateMatch(match, i, _engine);
            Assert.IsTrue(home <= 7, $"Goles local {home} superan el máximo");
            Assert.IsTrue(away <= 7, $"Goles visitante {away} superan el máximo");
        }
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldSetResultsForAllMatches()
    {
        var group = BuildGroupWithMatches();
        _service.SimulateGroupStage(new List<Group> { group }, Seed, _engine);
        foreach (var match in group.MatchesInGroup)
            Assert.IsTrue(match.HomeGoals.HasValue, $"El partido {match.IdMatch} no tiene resultado");
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldNotOverwriteResults()
    {
        var group = BuildGroupWithMatches();
        var primerPartido = group.MatchesInGroup[0];
        primerPartido.SetClock(new ClockStub(FechaResultadoClock));
        primerPartido.SetMatchResult(3, 0);

        _service.SimulateGroupStage(new List<Group> { group }, Seed, _engine);

        Assert.AreEqual(3, primerPartido.HomeGoals);
        Assert.AreEqual(0, primerPartido.AwayGoals);
    }

    [TestMethod]
    public void SimulateCruces_ShouldSetWinnerInCruce()
    {
        var cruce = BuildCruce("A1");
        _service.SimulateCruces(new List<Cruce> { cruce }, Seed, _engine);
        Assert.IsNotNull(cruce.GetWinner());
    }

    [TestMethod]
    public void SimulateCruces_ShouldNotOverwriteResults()
    {
        var cruce = BuildCruce("B1");
        cruce.Match.SetClock(new ClockStub(FechaResultadoClock));
        cruce.Match.SetMatchResult(2, 1);

        _service.SimulateCruces(new List<Cruce> { cruce }, Seed, _engine);

        Assert.AreEqual(2, cruce.Match.HomeGoals);
        Assert.AreEqual(1, cruce.Match.AwayGoals);
    }

    [TestMethod]
    public void SimulateMatch_WithValidMatch_ShouldRegisterSimulationExecutedLog()
    {
        var match = BuildMatch("Uruguay", 1500, "Argentina", 1500);
        _service.SimulateMatch(match, Seed, _engine);

        var logs = _logRepository.GetAll();
        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("system", logs[0].UserEmail);
        Assert.AreEqual("Simulation", logs[0].Category);
        Assert.AreEqual("SimulationExecuted", logs[0].Action);
        StringAssert.Contains(logs[0].Detail, Seed.ToString());
        StringAssert.Contains(logs[0].Detail, "Probabilístico");
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldAddCardIncidentsToMatches()
    {
        var group = BuildGroupWithMatches();
        _service.SimulateGroupStage(new List<Group> { group }, Seed, _engine);
        foreach (var match in group.MatchesInGroup)
            Assert.IsTrue(match.Incidents.Count > 0);
    }
}