using Domain;

namespace DomainTests;

[TestClass]
public class GroupTests
{
    [TestMethod]
    public void CreateGroup_WithValidLetter_ShouldCreateGroup()
    {
        var group = new Group('A');
        Assert.AreEqual('A', group.GroupLetter);
    }

    [TestMethod]
    public void CreateGroup_WithInvalidLetter_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Group('M'));
    }

    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var group = new Group('A');
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 500);
        group.AddTeamToGroup(team);
        Assert.AreEqual(1, group.TeamsInGroup.Count);
        Assert.AreEqual(team, group.TeamsInGroup[0]);
    }
    
    [TestMethod]
    public void AddTeam_WithNullTeam_ShouldThrowException()
    {
        var group = new Group('A');
        Assert.ThrowsException<ArgumentNullException>(() => group.AddTeamToGroup(null));
    }
}