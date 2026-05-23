using Domain.Calculators;
using Domain.Entities;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class StandingCalculatorTests
{
    private int _nextId;

    [TestInitialize]
    public void Setup()
    {
        _nextId = 1;
    }

    private Match CreateMatch(DateTimeMatchStub clock, Stadium stadium,
        Team home, Team away, char group = 'A')
    {
        return new Match(_nextId++, clock.Now, stadium, home, away, group, clock);
    }

    private static (Group group, Team uruguay, Team argentina, Stadium stadium, DateTimeMatchStub clock) CreateBasicGroup()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        return (group, uruguay, argentina, stadium, clock);
    }

    [TestMethod]
    public void CalculateStandings_WithOneMatch_ShouldGenerateStandings()
    {
        var (group, uruguay, argentina, stadium, clock) = CreateBasicGroup();
        var match = CreateMatch(clock, stadium, uruguay, argentina);
        match.SetMatchResult(2, 1);
        group.AddMatchToGroup(match);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(2, standings.Count);
    }

    [TestMethod]
    public void CalculateStandings_WithHomeTeamWin_ShouldUpdatePoints()
    {
        var (group, uruguay, argentina, stadium, clock) = CreateBasicGroup();
        var match = CreateMatch(clock, stadium, uruguay, argentina);
        match.SetMatchResult(2, 1);
        group.AddMatchToGroup(match);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(3, standings.First(s => s.TeamInGroup == uruguay).PointsInGroup);
        Assert.AreEqual(0, standings.First(s => s.TeamInGroup == argentina).PointsInGroup);
    }

    [TestMethod]
    public void CalculateStandings_WithDraw_ShouldUpdateBothTeamsWithOnePoint()
    {
        var (group, uruguay, argentina, stadium, clock) = CreateBasicGroup();
        var match = CreateMatch(clock, stadium, uruguay, argentina);
        match.SetMatchResult(1, 1);
        group.AddMatchToGroup(match);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(1, standings.First(s => s.TeamInGroup == uruguay).PointsInGroup);
        Assert.AreEqual(1, standings.First(s => s.TeamInGroup == argentina).PointsInGroup);
    }

    [TestMethod]
    public void CalculateStandings_WithAwayTeamWin_ShouldUpdatePoints()
    {
        var (group, uruguay, argentina, stadium, clock) = CreateBasicGroup();
        var match = CreateMatch(clock, stadium, uruguay, argentina);
        match.SetMatchResult(0, 2);
        group.AddMatchToGroup(match);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(0, standings.First(s => s.TeamInGroup == uruguay).PointsInGroup);
        Assert.AreEqual(3, standings.First(s => s.TeamInGroup == argentina).PointsInGroup);
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

        var match1 = CreateMatch(clock, stadium, uruguay, argentina);
        match1.SetMatchResult(2, 1);
        var match2 = CreateMatch(clock, stadium, brasil, argentina);
        match2.SetMatchResult(3, 0);
        var match3 = CreateMatch(clock, stadium, uruguay, brasil);
        match3.SetMatchResult(0, 1);

        group.AddMatchToGroup(match1);
        group.AddMatchToGroup(match2);
        group.AddMatchToGroup(match3);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(brasil, standings[0].TeamInGroup);
        Assert.AreEqual(6, standings[0].PointsInGroup);
    }

    [TestMethod]
    public void CalculateStandings_WhenTeamsHaveSamePoints_ShouldOrderByGoalDifference()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        var france = new Team("France", new Confederation("UEFA"), 1900);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        group.AddTeamToGroup(france);

        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);

        var match1 = CreateMatch(clock, stadium, uruguay, argentina);
        match1.SetMatchResult(3, 0);
        var match2 = CreateMatch(clock, stadium, france, argentina);
        match2.SetMatchResult(1, 0);

        group.AddMatchToGroup(match1);
        group.AddMatchToGroup(match2);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(uruguay, standings[0].TeamInGroup);
        Assert.AreEqual(france, standings[1].TeamInGroup);
    }

    [TestMethod]
    public void CalculateStandings_WhenTeamsHaveSamePointsAndGoalDifference_ShouldOrderByGoalsFor()
    {
        var group = new Group('A');
        var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        var brasil = new Team("France", new Confederation("UEFA"), 1900);
        group.AddTeamToGroup(uruguay);
        group.AddTeamToGroup(argentina);
        group.AddTeamToGroup(brasil);

        var stadium = new Stadium("Centenario", "Montevideo", "Desc", 60000);
        var clock = new DateTimeMatchStub(DateTime.Now);

        var match1 = CreateMatch(clock, stadium, uruguay, argentina);
        match1.SetMatchResult(4, 2);
        var match2 = CreateMatch(clock, stadium, brasil, argentina);
        match2.SetMatchResult(3, 1);

        group.AddMatchToGroup(match1);
        group.AddMatchToGroup(match2);

        var standings = new StandingCalculator().CalculateStandings(group);

        Assert.AreEqual(uruguay, standings[0].TeamInGroup);
        Assert.AreEqual(brasil, standings[1].TeamInGroup);
    }

    [TestMethod]
    public void CalculateStandings_WithMatchWithoutResult_ShouldNotUpdateStandings()
    {
        var (group, uruguay, argentina, stadium, clock) = CreateBasicGroup();
        var match = CreateMatch(clock, stadium, uruguay, argentina);
        group.AddMatchToGroup(match);

        var standings = new StandingCalculator().CalculateStandings(group);

        var uruguayStanding = standings.First(s => s.TeamInGroup == uruguay);
        var argentinaStanding = standings.First(s => s.TeamInGroup == argentina);

        Assert.AreEqual(0, uruguayStanding.PointsInGroup);
        Assert.AreEqual(0, argentinaStanding.PointsInGroup);
        Assert.AreEqual(0, uruguayStanding.MatchesPlayed);
        Assert.AreEqual(0, argentinaStanding.MatchesPlayed);
    }

    [TestMethod]
    public void CalculateStandings_WithNullGroup_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new StandingCalculator().CalculateStandings(null));
    }
}