using Domain.Entities;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class GroupTests
{
    private static readonly DateTimeMatchStub DefaultClock = new DateTimeMatchStub(DateTime.Now);
    private static readonly Stadium DefaultStadium = new Stadium("Estadio", "Ciudad", "Desc", 30000);
    private int _nextId = 1;

    [TestInitialize]
    public void Setup()
    {
        _nextId = 1;
    }

    private Match CreateMatch(Team home, Team away, char group = 'A', Stadium? stadium = null)
    {
        return new Match(_nextId++, DefaultClock.Now, stadium ?? DefaultStadium, home, away, group, DefaultClock);
    }

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
        group.AddTeamToGroup(new Team("Team2", new Confederation("UEFA"), 500));
        group.AddTeamToGroup(new Team("Team3", new Confederation("UEFA"), 500));
        group.AddTeamToGroup(new Team("Team4", new Confederation("UEFA"), 500));
        Assert.ThrowsException<InvalidOperationException>(() =>
            group.AddTeamToGroup(new Team("Team5", new Confederation("UEFA"), 500)));
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
        var stadium = new Stadium("Estadio Centenario", "Montevideo", "Desc", 60000);

        group.AddTeamToGroup(homeTeam);
        group.AddTeamToGroup(awayTeam);

        var match = CreateMatch(homeTeam, awayTeam, 'A', stadium);
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
        var match = CreateMatch(team1, team2, 'B');
        Assert.ThrowsException<ArgumentException>(() => group.AddMatchToGroup(match));
    }

    [TestMethod]
    public void AddMatch_WhenGroupHasSixMatches_ShouldThrowException()
    {
        var group = new Group('A');
        var team1 = new Team("A", new Confederation("UEFA"), 1000);
        var team2 = new Team("B", new Confederation("UEFA"), 1000);
        group.AddTeamToGroup(team1);
        group.AddTeamToGroup(team2);

        for (int i = 0; i < 6; i++)
            group.AddMatchToGroup(CreateMatch(team1, team2));

        Assert.ThrowsException<InvalidOperationException>(() =>
            group.AddMatchToGroup(CreateMatch(team1, team2)));
    }

    [TestMethod]
    public void AddMatch_WhenHomeTeamNotInGroup_ShouldThrowException()
    {
        var group = new Group('A');
        var teamInGroup = new Team("A", new Confederation("UEFA"), 1000);
        var teamOutsideGroup = new Team("B", new Confederation("UEFA"), 1000);
        group.AddTeamToGroup(teamInGroup);
        var match = CreateMatch(teamOutsideGroup, teamInGroup);
        Assert.ThrowsException<InvalidOperationException>(() => group.AddMatchToGroup(match));
    }

    [TestMethod]
    public void AddMatch_WhenAwayTeamNotInGroup_ShouldThrowException()
    {
        var group = new Group('A');
        var teamInGroup = new Team("A", new Confederation("UEFA"), 1000);
        var teamOutside = new Team("B", new Confederation("UEFA"), 1000);
        group.AddTeamToGroup(teamInGroup);
        var match = CreateMatch(teamInGroup, teamOutside);
        Assert.ThrowsException<InvalidOperationException>(() => group.AddMatchToGroup(match));
    }

    [TestMethod]
    public void AddMatch_WithDuplicateMatch_ShouldThrowException()
    {
        var group = new Group('A');
        var homeTeam = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var awayTeam = new Team("Argentina", new Confederation("CONMEBOL"), 1850);
        var stadium = new Stadium("Estadio Centenario", "Montevideo", "Desc", 60000);
        group.AddTeamToGroup(homeTeam);
        group.AddTeamToGroup(awayTeam);

        var match = CreateMatch(homeTeam, awayTeam, 'A', stadium);
        group.AddMatchToGroup(match);

        Assert.ThrowsException<InvalidOperationException>(() => group.AddMatchToGroup(match));
    }
    
    [TestMethod]
    public void RemoveTeamFromGroup_WithExistingTeam_ShouldRemoveTeam()
    {
        var group = new Group('A');
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        group.AddTeamToGroup(team);

        group.RemoveTeamFromGroup(team);

        Assert.AreEqual(0, group.TeamsInGroup.Count);
    }
    
    [TestMethod]
    public void RemoveTeamFromGroup_WithNullTeam_ShouldThrowException()
    {
        var group = new Group('A');

        Assert.ThrowsException<ArgumentNullException>(() =>
            group.RemoveTeamFromGroup(null!));
    }
    
    [TestMethod]
    public void RemoveTeamFromGroup_WithTeamNotInGroup_ShouldThrowException()
    {
        var group = new Group('A');
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        Assert.ThrowsException<InvalidOperationException>(() =>
            group.RemoveTeamFromGroup(team));
    }
    
    [TestMethod]
    public void PrivateConstructor_ShouldCreateGroup()
    {
        var group = (Group)Activator.CreateInstance(
            typeof(Group),
            nonPublic: true)!;

        Assert.IsNotNull(group);
    }
    
    [TestMethod]
    public void LoadMatchFromRepository_WithNewMatch_ShouldAddMatch()
    {
        var group = new Group('A');

        var home = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var away = new Team("Argentina", new Confederation("CONMEBOL"), 1800);

        var match = CreateMatch(home, away);

        group.LoadMatchFromRepository(match);

        Assert.AreEqual(1, group.MatchesInGroup.Count);
    }
    
    [TestMethod]
    public void LoadMatchFromRepository_WithExistingMatch_ShouldNotDuplicateMatch()
    {
        var group = new Group('A');

        var home = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var away = new Team("Argentina", new Confederation("CONMEBOL"), 1800);

        var match = CreateMatch(home, away);

        group.LoadMatchFromRepository(match);
        group.LoadMatchFromRepository(match);

        Assert.AreEqual(1, group.MatchesInGroup.Count);
    }
    
    [TestMethod]
    public void GetTeamsInGroup_ShouldReturnDistinctTeamsFromMatches()
    {
        var group = new Group('A');

        var team1 = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var team2 = new Team("Argentina", new Confederation("CONMEBOL"), 1800);

        group.LoadMatchFromRepository(CreateMatch(team1, team2));
        group.LoadMatchFromRepository(CreateMatch(team2, team1));

        var teams = group.GetTeamsInGroup();

        Assert.AreEqual(2, teams.Count);
    }
}