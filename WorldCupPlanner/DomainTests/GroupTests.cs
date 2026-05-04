using Domain;
using DomainTests.Stubs;

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

    [TestMethod]
    public void AddTeam_WhenGroupHasFourTeams_ShouldThrowException()
    {
        var group = new Group('A');
        group.AddTeamToGroup(new Team("Team1", new Confederation("UEFA"), 500));
        group.AddTeamToGroup(new Team("Team2", new Confederation("UEFA"),500));
        group.AddTeamToGroup(new Team("Team3", new Confederation("UEFA"),500));
        group.AddTeamToGroup(new Team("Team4", new Confederation("UEFA"), 500));
        Assert.ThrowsException<InvalidOperationException>(() => group.AddTeamToGroup(new Team("Team5", new Confederation("UEFA"), 500)));
    }

    [TestMethod]
    public void AddTeam_WithDuplicateTeam_ShouldThrowException()
    {
        var group = new Group('A');
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        group.AddTeamToGroup(team);
        Assert.ThrowsException<InvalidOperationException>(() => group.AddTeamToGroup(team));
    }

    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldAddMatch()
    {
        var group = new Group('A');
        var homeTeam = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var awayTeam = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var stadium = new Stadium("Estadio Centenario", "Montevideo", "Estadio mundialista", 60000);
        var match = new Match(clock.Now, stadium, homeTeam, awayTeam, 'A', clock);
        group.AddMatchToGroup(match);
        Assert.AreEqual(1, group.MatchesInGroup.Count);
        Assert.AreEqual(match, group.MatchesInGroup[0]);
    }

    [TestMethod]
    public void AddMatch_WithNullMatch_ShouldThrowException()
    {
        var group = new Group('A');
        Assert.ThrowsException<ArgumentNullException>(() => group.AddMatchToGroup(null));
    }

    [TestMethod]
    public void AddMatch_WithDifferentGroup_ShouldThrowException()
    {
        var group = new Group('A');
        var team1 = new Team("A", new Confederation("UEFA"), 1000);
        var team2 = new Team("B", new Confederation("UEFA"), 1000);
        var stadium = new Stadium("Estadio", "Ciudad", "Descripcion", 30000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        var match = new Match(clock.Now, stadium, team1, team2, 'B', clock);
        Assert.ThrowsException<ArgumentException>(() => group.AddMatchToGroup(match));
    }

    [TestMethod]
    public void AddMatch_WhenGroupHasSixMatches_ShouldThrowException()
    {
        var group = new Group('A');
        var team1 = new Team("A", new Confederation("UEFA"), 1000);
        var team2 = new Team("B", new Confederation("UEFA"), 1000);
        var stadium = new Stadium("Estadio", "Ciudad", "Descripcion", 30000);
        var clock = new DateTimeMatchStub(DateTime.Now);
        for (int i = 0; i < 6; i++)
        {
            var match = new Match(clock.Now, stadium, team1, team2, 'A', clock);
            group.AddMatchToGroup(match);
        }
        var extraMatch = new Match(clock.Now, stadium, team1, team2, 'A', clock);
        Assert.ThrowsException<InvalidOperationException>(() => group.AddMatchToGroup(extraMatch));
    }
}