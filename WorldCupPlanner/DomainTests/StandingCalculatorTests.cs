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
    
    [TestMethod]
    public void CalculateStandings_WithDraw_ShouldUpdateBothTeamsWithOnePoint()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        var match = new Match(clock.Now, stadium, uruguay, argentina, 'A', clock);
        match.SetMatchResult(1, 1);
        group.AddMatchToGroup(match);
        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        Standing uruguayStanding = standings.First(s => s.TeamInGroup == uruguay);
        Standing argentinaStanding = standings.First(s => s.TeamInGroup == argentina);
        Assert.AreEqual(1, uruguayStanding.PointsInGroup);
        Assert.AreEqual(1, argentinaStanding.PointsInGroup);
    }
    
    [TestMethod]
    public void CalculateStandings_WithAwayTeamWin_ShouldUpdatePoints()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        var match = new Match(clock.Now, stadium, uruguay, argentina, 'A', clock);
        match.SetMatchResult(0, 2);
        group.AddMatchToGroup(match);
        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        Standing uruguayStanding = standings.First(s => s.TeamInGroup == uruguay);
        Standing argentinaStanding = standings.First(s => s.TeamInGroup == argentina);
        Assert.AreEqual(0, uruguayStanding.PointsInGroup);
        Assert.AreEqual(3, argentinaStanding.PointsInGroup);
    }
    
    [TestMethod]
    public void CalculateStandings_ShouldOrderByPointsDescending()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        var brasil = new Team("France", new Confederation("CONMEBOL"), 1900);
        
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        group.AddTeamToGroup(brasil);

        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);

        var match1 = new Match(clock.Now, stadium, uruguay, argentina, 'A', clock);
        match1.SetMatchResult(2, 1);

        var match2 = new Match(clock.Now, stadium, brasil, argentina, 'A', clock);
        match2.SetMatchResult(3, 0);

        var match3 = new Match(clock.Now, stadium, uruguay, brasil, 'A', clock);
        match3.SetMatchResult(0, 1);

        group.AddMatchToGroup(match1);
        group.AddMatchToGroup(match2);
        group.AddMatchToGroup(match3);

        var calculator = new StandingCalculator();

        var standings = calculator.CalculateStandings(group);

        Assert.AreEqual(brasil, standings[0].TeamInGroup);
        Assert.AreEqual(6, standings[0].PointsInGroup);
    }
}