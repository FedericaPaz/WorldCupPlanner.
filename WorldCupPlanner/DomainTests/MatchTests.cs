using Domain.Entities;
using Domain.Enums;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class MatchTests
{
    private static readonly DateTime DefaultDate = new DateTime(2026, 6, 1);
    private static readonly DateTime FutureDate = new DateTime(2026, 6, 10);

    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("CONMEBOL"), 1500);
    }

    private Stadium CreateStadium(string name)
    {
        return new Stadium(name, "Montevideo", "", 60000);
    }

    private Match CreateMatch(
        DateTimeMatchStub? clock = null,
        string home = "Uruguay",
        string away = "Argentina",
        string stadium = "Centenario",
        char group = 'A',
        int id = 1,
        DateTime? date = null)
    {
        clock ??= new DateTimeMatchStub(DefaultDate);

        return new Match(
            id,
            date ?? clock.Now,
            CreateStadium(stadium),
            CreateTeam(home),
            CreateTeam(away),
            group,
            clock);
    }

    [TestMethod]
    public void CreateMatch_WithValidData_ShouldCreateMatch()
    {
        var clock = new DateTimeMatchStub(DefaultDate);
        var stadium = CreateStadium("Centenario");
        var homeTeam = CreateTeam("Uruguay");
        var awayTeam = CreateTeam("Argentina");

        var match = new Match(1, clock.Now, stadium, homeTeam, awayTeam, 'A', clock);

        Assert.AreEqual(1, match.IdMatch);
        Assert.AreEqual(clock.Now, match.DateMatch);
        Assert.AreEqual(stadium, match.Stadium);
        Assert.AreEqual(homeTeam, match.HomeTeam);
        Assert.AreEqual(awayTeam, match.AwayTeam);
        Assert.AreEqual('A', match.Group);
        Assert.IsNull(match.HomeGoals);
        Assert.IsNull(match.AwayGoals);
        Assert.IsNull(match.WinnerMatch);
    }

    [TestMethod]
    public void CreateMatch_WithDifferentIds_ShouldStoreCorrectId()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        var match1 = new Match(
            1,
            clock.Now,
            CreateStadium("Centenario"),
            CreateTeam("Uruguay"),
            CreateTeam("Argentina"),
            'A',
            clock);

        var match2 = new Match(
            2,
            clock.Now,
            CreateStadium("Maracana"),
            CreateTeam("Brasil"),
            CreateTeam("Peru"),
            'A',
            clock);

        Assert.AreEqual(1, match1.IdMatch);
        Assert.AreEqual(2, match2.IdMatch);
    }

    [TestMethod]
    public void CreateMatch_WithInvalidGroup_ShouldThrowArgumentException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                1,
                clock.Now,
                CreateStadium("Centenario"),
                CreateTeam("Uruguay"),
                CreateTeam("Argentina"),
                'M',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithSameTeams_ShouldThrowArgumentException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);
        var team = CreateTeam("Uruguay");

        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                1,
                clock.Now,
                CreateStadium("Centenario"),
                team,
                team,
                'A',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithNullStadium_ShouldThrowArgumentNullException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                1,
                clock.Now,
                null!,
                CreateTeam("Uruguay"),
                CreateTeam("Argentina"),
                'A',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithNullHomeTeam_ShouldThrowArgumentNullException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                1,
                clock.Now,
                CreateStadium("Centenario"),
                null!,
                CreateTeam("Argentina"),
                'A',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithNullAwayTeam_ShouldThrowArgumentNullException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                1,
                clock.Now,
                CreateStadium("Centenario"),
                CreateTeam("Uruguay"),
                null!,
                'A',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithNullGroup_ShouldThrowArgumentException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                1,
                clock.Now,
                CreateStadium("Centenario"),
                CreateTeam("Uruguay"),
                CreateTeam("Argentina"),
                '\0',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithNullClock_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Match(
                1,
                DefaultDate,
                CreateStadium("Centenario"),
                CreateTeam("Uruguay"),
                CreateTeam("Argentina"),
                'A',
                null!));
    }

    [TestMethod]
    public void CreateMatch_WithPastDate_ShouldThrowArgumentException()
    {
        var clock = new DateTimeMatchStub(FutureDate);

        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                1,
                DefaultDate,
                CreateStadium("Centenario"),
                CreateTeam("Uruguay"),
                CreateTeam("Argentina"),
                'A',
                clock));
    }

    [TestMethod]
    public void CreateMatch_WithLowercaseGroup_ShouldSaveGroupAsUppercase()
    {
        var match = CreateMatch(group: 'a');

        Assert.AreEqual('A', match.Group);
    }

    [TestMethod]
    public void CreateMatch_WithDifferentTeamObjectsButSameName_ShouldThrowArgumentException()
    {
        var clock = new DateTimeMatchStub(DefaultDate);

        Assert.ThrowsException<ArgumentException>(() =>
            new Match(
                1,
                clock.Now,
                CreateStadium("Centenario"),
                CreateTeam("Uruguay"),
                CreateTeam("Uruguay"),
                'A',
                clock));
    }

    [TestMethod]
    public void SetResult_WithValidGoals_ShouldAssignGoalsAndWinner()
    {
        var clock = new DateTimeMatchStub(DefaultDate);
        var home = CreateTeam("Uruguay");

        var match = new Match(
            1,
            clock.Now,
            CreateStadium("Centenario"),
            home,
            CreateTeam("Argentina"),
            'A',
            clock);

        match.SetMatchResult(2, 1);

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
        Assert.AreEqual(home, match.WinnerMatch);
    }

    [TestMethod]
    public void SetResult_WithNegativeGoals_ShouldThrowException()
    {
        var match = CreateMatch();

        Assert.ThrowsException<ArgumentException>(() =>
            match.SetMatchResult(-1, 1));
    }

    [TestMethod]
    public void SetResult_WhenAlreadySet_ShouldThrowInvalidOperationException()
    {
        var match = CreateMatch();

        match.SetMatchResult(1, 0);

        Assert.ThrowsException<InvalidOperationException>(() =>
            match.SetMatchResult(2, 0));
    }

    [TestMethod]
    public void SetMatchResult_WithAwayTeamWinner_ShouldAssignAwayTeamAsWinner()
    {
        var clock = new DateTimeMatchStub(DefaultDate);
        var awayTeam = CreateTeam("Argentina");

        var match = new Match(
            1,
            clock.Now,
            CreateStadium("Centenario"),
            CreateTeam("Uruguay"),
            awayTeam,
            'A',
            clock);

        match.SetMatchResult(0, 2);

        Assert.AreEqual(awayTeam, match.WinnerMatch);
    }

    [TestMethod]
    public void SetMatchResult_WithDraw_ShouldLeaveWinnerNull()
    {
        var match = CreateMatch();

        match.SetMatchResult(1, 1);

        Assert.IsNull(match.WinnerMatch);
    }

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldUpdateDate()
    {
        var match = CreateMatch();
        var newDate = new DateTime(2026, 6, 5);

        match.UpdateDate(newDate);

        Assert.AreEqual(newDate, match.DateMatch);
    }

    [TestMethod]
    public void UpdateDate_WithPastDate_ShouldThrowArgumentException()
    {
        var clock = new DateTimeMatchStub(FutureDate);

        var match = new Match(
            1,
            clock.Now,
            CreateStadium("Centenario"),
            CreateTeam("Uruguay"),
            CreateTeam("Argentina"),
            'A',
            clock);

        Assert.ThrowsException<ArgumentException>(() =>
            match.UpdateDate(DefaultDate));
    }

    [TestMethod]
    public void UpdateStadium_WithValidStadium_ShouldUpdateStadium()
    {
        var match = CreateMatch();
        var newStadium = CreateStadium("Maracana");

        match.UpdateStadium(newStadium);

        Assert.AreEqual(newStadium, match.Stadium);
    }

    [TestMethod]
    public void UpdateStadium_WithNullStadium_ShouldThrowArgumentNullException()
    {
        var match = CreateMatch();

        Assert.ThrowsException<ArgumentNullException>(() =>
            match.UpdateStadium(null!));
    }

    [TestMethod]
    public void AddIncident_WithValidIncident_ShouldAddIncidentToMatch()
    {
        var now = new DateTime(2026, 6, 1, 10, 0, 0);
        var clock = new DateTimeMatchStub(now);

        var homeTeam = CreateTeam("Uruguay");
        var awayTeam = CreateTeam("España");
        var stadium = CreateStadium("Centenario");

        var match = new Match(
            1,
            now.AddDays(1),
            stadium,
            homeTeam,
            awayTeam,
            'A',
            clock);

        var incident = new MatchIncident(
            IncidentType.YellowCard,
            homeTeam,
            1);

        match.AddIncident(incident);

        Assert.AreEqual(1, match.Incidents.Count);
    }

    [TestMethod]
    public void AddIncident_WithNullIncident_ShouldThrowArgumentNullException()
    {
        var now = new DateTime(2026, 6, 1, 10, 0, 0);
        var clock = new DateTimeMatchStub(now);

        var homeTeam = CreateTeam("Uruguay");
        var awayTeam = CreateTeam("España");
        var stadium = CreateStadium("Centenario");

        var match = new Match(
            1,
            now.AddDays(1),
            stadium,
            homeTeam,
            awayTeam,
            'A',
            clock);

        Assert.ThrowsException<ArgumentNullException>(() =>
            match.AddIncident(null!));
    }
}