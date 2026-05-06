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
}