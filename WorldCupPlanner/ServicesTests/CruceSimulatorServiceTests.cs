using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class CruceSimulatorServiceTests
{
    private CruceSimulationService _service;
    private const int Seed = 42;

    [TestInitialize]
    public void Setup()
    {
        _service = new CruceSimulationService();
    }
    
    private Match BuildMatch(string homeName, int homeRanking, string awayName, int awayRanking, char group = 'A')
    {
        var home    = new Team(homeName, new Confederation("UEFA"), homeRanking);
        var away    = new Team(awayName, new Confederation("UEFA"), awayRanking);
        var stadium = new Stadium("Estadio Test", "Ciudad", "", 50000);
        var clock   = new ClockStub(DateTime.Now.AddHours(2));
        return new Match(DateTime.Now, stadium, home, away, group, clock);
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
}