using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class CruceSimulatorServiceTests
{
    private CrucesSimulationService _service;
    private const int Seed = 42;

    [TestInitialize]
    public void Setup()
    {
        _service = new CrucesSimulationService();
    }
    
    private Match BuildMatch(string homeName, int homeRanking, string awayName, int awayRanking, char group = 'A')
    {
        var home    = new Team(homeName, new Confederation("UEFA"), homeRanking);
        var away    = new Team(awayName, new Confederation("UEFA"), awayRanking);
        var stadium = new Stadium("Estadio Test", "Ciudad", "", 50000);
        var clock   = new ClockStub(DateTime.Now.AddHours(2));
        return new Match(DateTime.Now, stadium, home, away, group, clock);
    }
    
    private Group BuildGroupWithMatches()
    {
        var teams = new[]
        {
            new Team("TeamA", new Confederation("UEFA"),   1800),
            new Team("TeamB", new Confederation("UEFA"),   1700),
            new Team("TeamC", new Confederation("CONMEBOL"), 1600),
            new Team("TeamD", new Confederation("CAF"),    1500),
        };

        var stadium = new Stadium("Est1", "Ciudad", "", 50000);
        var clock   = new ClockStub(DateTime.Now.AddHours(2));
        var group   = new Group('A');

        foreach (var t in teams)
            group.AddTeamToGroup(t);

        var matchups = new[] { (0,3),(1,2),(0,2),(1,3),(0,1),(2,3) };
        foreach (var (h, a) in matchups)
        {
            var m = new Match(DateTime.Now, stadium, teams[h], teams[a], 'A', clock);
            group.AddMatchToGroup(m);
        }
        return group;
    }
    
    private Cruce BuildCruce(string nombre)
    {
        var home    = new Team("Alemania",  new Confederation("UEFA"),     2000);
        var away    = new Team("Australia", new Confederation("AFC"),       400);
        var stadium = new Stadium("Test Stadium2", "Ciudad", "", 50000);
        var clock   = new ClockStub(DateTime.Now.AddHours(2));
        var match   = new Match(DateTime.Now, stadium, home, away, 'A', clock);
        return new Cruce(nombre, match, TournamentPhase.RoundOf32);
    }
    
    [TestMethod]
    public void SimulateMatch_GoalsShouldNotBeNegative()
    {
        Match.ResetIdCounter();
        var match = BuildMatch("Argentina", 1800, "Bolivia", 500);
        var (home, away) = _service.SimulateMatch(match, Seed);
        Assert.IsTrue(home >= 0);
        Assert.IsTrue(away >= 0);
    }
    
    [TestMethod]
    public void SimulateMatch_WithSameSeed_ShouldReturnSameResult()
    {
        Match.ResetIdCounter();
        var match1 = BuildMatch("España", 1900, "Peru", 700);
        Match.ResetIdCounter();
        var match2 = BuildMatch("España", 1900, "Peru", 700);
        var result1 = _service.SimulateMatch(match1, Seed);
        var result2 = _service.SimulateMatch(match2, Seed);
        Assert.AreEqual(result1.homeGoals, result2.homeGoals);
        Assert.AreEqual(result1.awayGoals, result2.awayGoals);
    }
    
    [TestMethod]
    public void SimulateMatch_DifferentSeeds_ShouldReturnDifferentResults()
    {
        Match.ResetIdCounter();
        var match1 = BuildMatch("Francia", 2000, "Ecuador", 600);
        Match.ResetIdCounter();
        var match2 = BuildMatch("Francia", 2000, "Ecuador", 600);
        var result1 = _service.SimulateMatch(match1, seed: 1);
        var result2 = _service.SimulateMatch(match2, seed: 99999);
        bool sonDistintos = result1.homeGoals != result2.homeGoals || result1.awayGoals != result2.awayGoals;
        Assert.IsTrue(sonDistintos);
    }
    
    [TestMethod]
    public void SimulateMatch_GoalsShouldNotExceedMaximum()
    {
        for (int i = 0; i < 50; i++)
        {
            Match.ResetIdCounter();
            var match = BuildMatch($"Team{i}A", 1000 + i, $"Team{i}B", 1000 + i + 1);
            var (home, away) = _service.SimulateMatch(match, seed: i);
            Assert.IsTrue(home <= 7, $"Goles local {home} superan el máximo");
            Assert.IsTrue(away <= 7, $"Goles visitante {away} superan el máximo");
        }
    }
    
    [TestMethod]
    public void SimulateGroupStage_ShouldSetResultsForAllMatches()
    {
        Match.ResetIdCounter();
        var group = BuildGroupWithMatches();
        _service.SimulateGroupStage(new List<Group> { group }, Seed);
        foreach (var match in group.MatchesInGroup)
            Assert.IsTrue(match.HomeGoals.HasValue, $"El partido {match.IdMatch} no tiene resultado");
    }
    
    [TestMethod]
    public void SimulateGroupStage_ShouldNotOverwriteResults()
    {
        Match.ResetIdCounter();
        var group = BuildGroupWithMatches();
        var primerPartido = group.MatchesInGroup[0];
        primerPartido.SetMatchResult(3, 0);
        _service.SimulateGroupStage(new List<Group> { group }, Seed);
        Assert.AreEqual(3, primerPartido.HomeGoals);
        Assert.AreEqual(0, primerPartido.AwayGoals);
    }
    
    [TestMethod]
    public void SimulateCruces_ShouldSetWinnerInCruce()
    {
        Match.ResetIdCounter();
        var cruce = BuildCruce("A1");
        _service.SimulateCruces(new List<Cruce> { cruce }, Seed);
        Assert.IsNotNull(cruce.GetWinner());
    }
    
    [TestMethod]
    public void SimulateCruces_ShouldNotOverwriteResults()
    {
        Match.ResetIdCounter();
        var cruce = BuildCruce("B1");
        cruce.Partido.SetMatchResult(2, 1);
        _service.SimulateCruces(new List<Cruce> { cruce }, Seed);
        Assert.AreEqual(2, cruce.Partido.HomeGoals);
        Assert.AreEqual(1, cruce.Partido.AwayGoals);
    }
}