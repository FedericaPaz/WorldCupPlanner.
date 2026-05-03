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
    
    [TestMethod]
    public void Create_WithValidRequest_ShouldCreateMatch()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        var response = service.Create(request);

        Assert.AreEqual(1, response.idMatch);
        Assert.AreEqual("Centenario", response.stadiumName);
        Assert.AreEqual("Uruguay", response.homeTeamName);
        Assert.AreEqual("Argentina", response.awayTeamName);
        Assert.AreEqual('A', response.group);
    }
    
    [TestMethod]
    public void Create_WithNullRequest_ShouldThrowArgumentNullException()
    {
        var service = new MatchService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));
        Assert.ThrowsException<ArgumentNullException>(() => service.Create(null));
    }
    
    [TestMethod]
    public void Create_WithValidRequest_ShouldStoreMatchInRepository()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        service.Create(request);
        Assert.AreEqual(1, matchRepository.GetAllMatches().Count);
    }
    
    [TestMethod]
    public void Create_WithEmptyGroup_ShouldThrowArgumentException()
    {
        var service = new MatchService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));

        var request = new MatchRequest
        {
            dateMatch = DateTime.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "" 
        };

        Assert.ThrowsException<ArgumentException>(() => service.Create(request));
    }
}