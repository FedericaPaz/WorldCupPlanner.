using Services;
using Domain.Entities;
using Domain.Enums;
using ServicesTests.Stubs;
using Repository;

namespace ServicesTests;

[TestClass]
public class CruceSimulatorServiceTests
{
    private static readonly DateTime FechaCreacionClock  = new DateTime(2027, 6, 10);
    private static readonly DateTime FechaPartido        = new DateTime(2027, 6, 12);
    private static readonly DateTime FechaResultadoClock = new DateTime(2027, 6, 15);
    private LogRepository _logRepository = null!;
    private LogService _logService = null!;
    private CrucesSimulationService _service = null!;
    private const int Seed = 42;
    private int _nextId;

    [TestInitialize]
    public void Setup()
    {
        _nextId = 1;
        _logRepository = new LogRepository();
        _logService = new LogService(_logRepository);
        _service = new CrucesSimulationService(_logService);
    }

    // ── Helpers ───────────────────────────────────────────────────

    private Match BuildMatch(string homeName, int homeRanking,
        string awayName, int awayRanking, char group = 'A')
    {
        var home    = new Team(homeName, new Confederation("UEFA"), homeRanking);
        var away    = new Team(awayName, new Confederation("UEFA"), awayRanking);
        var stadium = new Stadium("Estadio Test", "Ciudad", "", 50000);
        return new Match(_nextId++, FechaPartido, stadium, home, away, group,
            new ClockStub(FechaCreacionClock));
    }

    private Group BuildGroupWithMatches()
    {
        var teams = new[]
        {
            new Team("TeamA", new Confederation("UEFA"),     1800),
            new Team("TeamB", new Confederation("UEFA"),     1700),
            new Team("TeamC", new Confederation("CONMEBOL"), 1600),
            new Team("TeamD", new Confederation("CAF"),      1500),
        };

        var stadium  = new Stadium("Est1", "Ciudad", "", 50000);
        var group    = new Group('A');
        foreach (var t in teams)
            group.AddTeamToGroup(t);

        var matchups = new[] { (0, 3), (1, 2), (0, 2), (1, 3), (0, 1), (2, 3) };
        foreach (var (h, a) in matchups)
        {
            var m = new Match(_nextId++, FechaPartido, stadium, teams[h], teams[a], 'A',
                new ClockStub(FechaCreacionClock));
            group.AddMatchToGroup(m);
        }
        return group;
    }

    private Cruce BuildCruce(string nombre)
    {
        var home    = new Team("Alemania",  new Confederation("UEFA"), 2000);
        var away    = new Team("Australia", new Confederation("AFC"),  400);
        var stadium = new Stadium("Test Stadium2", "Ciudad", "", 50000);
        var match   = new Match(_nextId++, FechaPartido, stadium, home, away, 'A',
            new ClockStub(FechaCreacionClock));
        return new Cruce(nombre, match, TournamentPhase.RoundOf32);
    }

    // ── Tests ─────────────────────────────────────────────────────

    [TestMethod]
    public void SimulateMatch_GoalsShouldNotBeNegative()
    {
        var match = BuildMatch("Argentina", 1800, "Bolivia", 500);
        var (home, away) = _service.SimulateMatch(match, Seed);
        Assert.IsTrue(home >= 0);
        Assert.IsTrue(away >= 0);
    }

    [TestMethod]
    public void SimulateMatch_WithSameSeed_ShouldReturnSameResult()
    {
        var match1 = new Match(1, FechaPartido,
            new Stadium("Estadio Test", "Ciudad", "", 50000),
            new Team("España", new Confederation("UEFA"), 1900),
            new Team("Peru",   new Confederation("UEFA"), 700),
            'A', new ClockStub(FechaCreacionClock));

        var match2 = new Match(1, FechaPartido,
            new Stadium("Estadio Test", "Ciudad", "", 50000),
            new Team("España", new Confederation("UEFA"), 1900),
            new Team("Peru",   new Confederation("UEFA"), 700),
            'A', new ClockStub(FechaCreacionClock));

        var result1 = _service.SimulateMatch(match1, Seed);
        var result2 = _service.SimulateMatch(match2, Seed);

        Assert.AreEqual(result1.homeGoals, result2.homeGoals);
        Assert.AreEqual(result1.awayGoals, result2.awayGoals);
    }

    [TestMethod]
    public void SimulateMatch_DifferentSeeds_ShouldReturnDifferentResults()
    {
        var match1  = BuildMatch("Francia", 2000, "Ecuador", 600);
        var match2  = BuildMatch("Francia", 2000, "Ecuador", 600);
        var result1 = _service.SimulateMatch(match1, seed: 1);
        var result2 = _service.SimulateMatch(match2, seed: 99999);
        Assert.IsTrue(result1.homeGoals != result2.homeGoals || result1.awayGoals != result2.awayGoals);
    }

    [TestMethod]
    public void SimulateMatch_GoalsShouldNotExceedMaximum()
    {
        for (int i = 0; i < 50; i++)
        {
            var match = BuildMatch($"Team{i}A", 1000 + i, $"Team{i}B", 1000 + i + 1);
            var (home, away) = _service.SimulateMatch(match, seed: i);
            Assert.IsTrue(home <= 7, $"Goles local {home} superan el máximo");
            Assert.IsTrue(away <= 7, $"Goles visitante {away} superan el máximo");
        }
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldSetResultsForAllMatches()
    {
        var group = BuildGroupWithMatches();
        _service.SimulateGroupStage(new List<Group> { group }, Seed);
        foreach (var match in group.MatchesInGroup)
            Assert.IsTrue(match.HomeGoals.HasValue, $"El partido {match.IdMatch} no tiene resultado");
    }

    [TestMethod]
    public void SimulateGroupStage_ShouldNotOverwriteResults()
    {
        var group        = BuildGroupWithMatches();
        var primerPartido = group.MatchesInGroup[0];
        primerPartido.SetClock(new ClockStub(FechaResultadoClock));
        primerPartido.SetMatchResult(3, 0);
        _service.SimulateGroupStage(new List<Group> { group }, Seed);
        Assert.AreEqual(3, primerPartido.HomeGoals);
        Assert.AreEqual(0, primerPartido.AwayGoals);
    }

    [TestMethod]
    public void SimulateCruces_ShouldSetWinnerInCruce()
    {
        var cruce = BuildCruce("A1");
        _service.SimulateCruces(new List<Cruce> { cruce }, Seed);
        Assert.IsNotNull(cruce.GetWinner());
    }

    [TestMethod]
    public void SimulateCruces_ShouldNotOverwriteResults()
    {
        var cruce = BuildCruce("B1");
        cruce.Match.SetClock(new ClockStub(FechaResultadoClock));
        cruce.Match.SetMatchResult(2, 1);
        _service.SimulateCruces(new List<Cruce> { cruce }, Seed);
        Assert.AreEqual(2, cruce.Match.HomeGoals);
        Assert.AreEqual(1, cruce.Match.AwayGoals);
    }
    
    [TestMethod]
    public void SimulateMatch_WithValidMatch_ShouldRegisterSimulationExecutedLog()
    {
        var match = BuildMatch("Uruguay", 1500, "Argentina", 1500);

        _service.SimulateMatch(match, Seed);

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("system", logs[0].UserEmail);
        Assert.AreEqual("Simulation", logs[0].Category);
        Assert.AreEqual("SimulationExecuted", logs[0].Action);
        StringAssert.Contains(logs[0].Detail, Seed.ToString());
        StringAssert.Contains(logs[0].Detail, "CrucesSimulationService");
    }
}