using DTOs;
using Domain;
using DomainTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class MatchServiceTests
{
    [TestInitialize]
    public void Setup()
    {
        Match.ResetIdCounter();
    }

    private static Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("CONMEBOL"), 1900);
    }

    private static Stadium CreateStadium(string name)
    {
        return new Stadium(name, "Montevideo", null, 60000);
    }

    private static MatchRequest CreateValidRequest(DateTime date)
    {
        return new MatchRequest
        {
            dateMatch = date,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };
    }

    private static MatchService CreateService(
        MatchRepository matchRepository,
        TeamRepositoryStub teamRepository,
        StadiumRepositoryStub stadiumRepository,
        DateTimeMatchStub clock)
    {
        return new MatchService(matchRepository, teamRepository, stadiumRepository, clock);
    }

    private static void AddDefaultTeamsAndStadium(
        TeamRepositoryStub teamRepository,
        StadiumRepositoryStub stadiumRepository)
    {
        teamRepository.Add(CreateTeam("Uruguay"));
        teamRepository.Add(CreateTeam("Argentina"));
        stadiumRepository.AddStadium(CreateStadium("Centenario"));
    }

    [TestMethod]
    public void Create_WithValidRequest_ShouldCreateMatch()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var response = service.CreateMatch(CreateValidRequest(clock.Now));

        Assert.AreEqual(1, response.idMatch);
        Assert.AreEqual("Centenario", response.stadiumName);
        Assert.AreEqual("Uruguay", response.homeTeamName);
        Assert.AreEqual("Argentina", response.awayTeamName);
        Assert.AreEqual('A', response.group);
    }

    [TestMethod]
    public void Create_WithNullRequest_ShouldThrowArgumentNullException()
    {
        var service = CreateService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));

        Assert.ThrowsException<ArgumentNullException>(() => service.CreateMatch(null));
    }

    [TestMethod]
    public void Create_WithValidRequest_ShouldStoreMatchInRepository()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        service.CreateMatch(CreateValidRequest(clock.Now));

        Assert.AreEqual(1, matchRepository.GetAllMatches().Count);
    }

    [TestMethod]
    public void Create_WithEmptyGroup_ShouldThrowArgumentException()
    {
        var service = CreateService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));

        var request = CreateValidRequest(DateTime.Now);
        request.group = "";

        Assert.ThrowsException<ArgumentException>(() => service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithNonExistingTeam_ShouldThrowKeyNotFoundException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(CreateTeam("Uruguay"));
        stadiumRepository.AddStadium(CreateStadium("Centenario"));

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = CreateValidRequest(clock.Now);

        Assert.ThrowsException<KeyNotFoundException>(() => service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithNonExistingStadium_ShouldThrowKeyNotFoundException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(CreateTeam("Uruguay"));
        teamRepository.Add(CreateTeam("Argentina"));

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = CreateValidRequest(clock.Now);

        Assert.ThrowsException<KeyNotFoundException>(() => service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithSameHomeAndAwayTeam_ShouldThrowArgumentException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(CreateTeam("Uruguay"));
        stadiumRepository.AddStadium(CreateStadium("Centenario"));

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = CreateValidRequest(clock.Now);
        request.awayTeamName = "Uruguay";

        Assert.ThrowsException<ArgumentException>(() => service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithPastDate_ShouldThrowArgumentException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 10));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = CreateValidRequest(new DateTime(2026, 6, 1));

        Assert.ThrowsException<ArgumentException>(() => service.CreateMatch(request));
    }

    [TestMethod]
    public void SetResult_WithValidData_ShouldUpdateMatchResult()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        service.SetMatchResult(created.idMatch, 2, 1);

        var match = matchRepository.GetMatchById(created.idMatch);

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }

    [TestMethod]
    public void SetResult_WithNegativeGoals_ShouldThrowArgumentException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        Assert.ThrowsException<ArgumentException>(() => service.SetMatchResult(created.idMatch, -1, 0));
    }

    [TestMethod]
    public void SetResult_WhenMatchNotPlayedYet_ShouldThrowInvalidOperationException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(new DateTime(2026, 6, 10)));

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.SetMatchResult(created.idMatch, 2, 1));
    }

    [TestMethod]
    public void SetResult_WhenResultAlreadySet_ShouldThrowInvalidOperationException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        service.SetMatchResult(created.idMatch, 1, 0);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.SetMatchResult(created.idMatch, 2, 0));
    }

    [TestMethod]
    public void GetById_WithExistingMatch_ShouldReturnMatchResponse()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        var result = service.GetMatchById(created.idMatch);

        Assert.AreEqual(created.idMatch, result.idMatch);
        Assert.AreEqual("Centenario", result.stadiumName);
        Assert.AreEqual("Uruguay", result.homeTeamName);
        Assert.AreEqual("Argentina", result.awayTeamName);
    }

    [TestMethod]
    public void GetById_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        var service = CreateService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));

        Assert.ThrowsException<KeyNotFoundException>(() => service.GetMatchById(99));
    }

    [TestMethod]
    public void GetAll_WhenThereAreNoMatches_ShouldReturnEmptyList()
    {
        var service = CreateService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));

        var result = service.GetAllMatches();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAll_WithMatches_ShouldReturnAllMatchResponses()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        teamRepository.Add(CreateTeam("Brasil"));
        teamRepository.Add(CreateTeam("Peru"));

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        service.CreateMatch(CreateValidRequest(clock.Now));

        service.CreateMatch(new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Brasil",
            awayTeamName = "Peru",
            group = "B"
        });

        var result = service.GetAllMatches();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Uruguay", result[0].homeTeamName);
        Assert.AreEqual("Brasil", result[1].homeTeamName);
    }

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldUpdateMatchDate()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        var newDate = new DateTime(2026, 6, 5);

        service.UpdateMatchDate(created.idMatch, newDate);

        var updated = service.GetMatchById(created.idMatch);

        Assert.AreEqual(newDate, updated.dateMatch);
    }

    [TestMethod]
    public void UpdateDate_WithPastDate_ShouldThrowArgumentException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 10));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        var pastDate = new DateTime(2026, 6, 1);

        Assert.ThrowsException<ArgumentException>(() =>
            service.UpdateMatchDate(created.idMatch, pastDate));
    }

    [TestMethod]
    public void UpdateStadium_WithValidData_ShouldUpdateStadium()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        stadiumRepository.AddStadium(CreateStadium("Maracana"));

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        service.UpdateMatchStadium(created.idMatch, "Maracana");

        var updated = service.GetMatchById(created.idMatch);

        Assert.AreEqual("Maracana", updated.stadiumName);
    }

    [TestMethod]
    public void UpdateStadium_WithNonExistingStadium_ShouldThrowKeyNotFoundException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        AddDefaultTeamsAndStadium(teamRepository, stadiumRepository);

        var service = CreateService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(CreateValidRequest(clock.Now));

        Assert.ThrowsException<KeyNotFoundException>(() =>
            service.UpdateMatchStadium(created.idMatch, "Maracana"));
    }
}