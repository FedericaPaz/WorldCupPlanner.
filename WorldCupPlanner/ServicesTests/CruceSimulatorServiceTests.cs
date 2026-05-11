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
}