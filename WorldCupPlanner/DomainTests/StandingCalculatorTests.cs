using Domain;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class StandingCalculatorTests
{
    [TestMethod]
    public void CalculateStandings_WithOneMatch_ShouldGenerateStandings()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        var match = new Match(clock.Now, stadium, uruguay, argentina, 'A', clock);
        match.SetMatchResult(2, 1);
        group.AddMatchToGroup(match);
        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        Assert.AreEqual(2, standings.Count);
    }
    
    [TestMethod]
    public void CalculateStandings_WithHomeTeamWin_ShouldUpdatePoints()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        var stadium = new Stadium("Centenario", "Montevideo", "Desc",60000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        var match = new Match(clock.Now, stadium, uruguay, argentina, 'A', clock);
        match.SetMatchResult(2, 1);
        group.AddMatchToGroup(match);
        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        Standing uruguayStanding = standings.First(s => s.TeamInGroup == uruguay);
        Standing argentinaStanding = standings.First(s => s.TeamInGroup == argentina);
        Assert.AreEqual(3, uruguayStanding.PointsInGroup);
        Assert.AreEqual(0, argentinaStanding.PointsInGroup);
    }
}