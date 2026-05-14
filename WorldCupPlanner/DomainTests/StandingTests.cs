using Domain;

namespace DomainTests;

[TestClass]
public class StandingTests
{
    [TestMethod]
    public void CreateStanding_WithValidTeam_ShouldCreateStanding()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        Assert.AreEqual(team, standing.TeamInGroup);
        Assert.AreEqual(0, standing.PointsInGroup);
        Assert.AreEqual(0, standing.GoalsFor);
        Assert.AreEqual(0, standing.GoalsAgainst);
    }

    [TestMethod]
    public void CreateStanding_WithNullTeam_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new Standing(null));
    }

    [TestMethod]
    public void AddMatchResult_WhenTeamWins_ShouldAddThreePoints()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(2, 1);
        Assert.AreEqual(3, standing.PointsInGroup);
    }

    [TestMethod]
    public void AddMatchResult_WhenTeamDraws_ShouldAddOnePoint()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(1, 1);
        Assert.AreEqual(1, standing.PointsInGroup);
    }

    [TestMethod]
    public void AddMatchResult_WhenTeamLoses_ShouldNotAddPoints()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(0, 1);
        Assert.AreEqual(0, standing.PointsInGroup);
    }

    [TestMethod]
    public void AddMatchResult_ShouldUpdateGoalsForAndGoalsAgainst()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(3, 1);
        Assert.AreEqual(3, standing.GoalsFor);
        Assert.AreEqual(1, standing.GoalsAgainst);
    }

    [TestMethod]
    public void GoalDifference_ShouldReturnGoalsForMinusGoalsAgainst()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(4, 1);
        Assert.AreEqual(3, standing.GoalDifference);
    }

    [TestMethod]
    public void AddMatchResult_ShouldIncrementMatchesPlayed()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(2, 1);
        Assert.AreEqual(1, standing.MatchesPlayed);
    }

    [TestMethod]
    public void AddMatchResult_WithNegativeGoals_ShouldThrowException()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => standing.AddMatchResult(-1, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => standing.AddMatchResult(0, -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => standing.AddMatchResult(-2, -3));
    }

    [TestMethod]
    public void AddMatchResult_WhenTeamWins_ShouldIncrementWins()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(2, 1);
        Assert.AreEqual(1, standing.Wins);
    }

    [TestMethod]
    public void AddMatchResult_WhenTeamDraws_ShouldIncrementDraws()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(1, 1);
        Assert.AreEqual(1, standing.Draws);
    }

    [TestMethod]
    public void AddMatchResult_WhenTeamLoses_ShouldIncrementLoses()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(0, 2);
        Assert.AreEqual(1, standing.Losses);
    }
    
    [TestMethod]
    public void AddMatchResult_WithMultipleMatches_ShouldAccumulateStatistics()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var standing = new Standing(team);
        standing.AddMatchResult(2, 1);
        standing.AddMatchResult(1, 1);
        standing.AddMatchResult(0, 3);
        Assert.AreEqual(4, standing.PointsInGroup);
        Assert.AreEqual(3, standing.GoalsFor);
        Assert.AreEqual(5, standing.GoalsAgainst);
        Assert.AreEqual(-2, standing.GoalDifference);
        Assert.AreEqual(1, standing.Wins);
        Assert.AreEqual(1, standing.Draws);
        Assert.AreEqual(1, standing.Losses);
        Assert.AreEqual(3, standing.MatchesPlayed);
    }
}