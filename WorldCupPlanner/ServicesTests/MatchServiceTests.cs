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

        var response = service.CreateMatch(request);

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
        Assert.ThrowsException<ArgumentNullException>(() => service.CreateMatch(null));
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

        service.CreateMatch(request);
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

        Assert.ThrowsException<ArgumentException>(() => service.CreateMatch(request));
    }
    
    [TestMethod]
    public void Create_WithNonExistingTeam_ShouldThrowKeyNotFoundException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        teamRepository.Add(new TeamStub("Uruguay"));
        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina", // ← NO existe
            group = "A"
        };

        Assert.ThrowsException<KeyNotFoundException>(() => service.CreateMatch(request));
    }
    
    [TestMethod]
    public void Create_WithNonExistingStadium_ShouldThrowKeyNotFoundException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        
        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario", // ← NO existe
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        Assert.ThrowsException<KeyNotFoundException>(() => service.CreateMatch(request));
    }
    
    [TestMethod]
    public void Create_WithSameHomeAndAwayTeam_ShouldThrowArgumentException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(new TeamStub("Uruguay"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Uruguay",
            group = "A"
        };

        Assert.ThrowsException<ArgumentException>(() => service.CreateMatch(request));
    }
    
    [TestMethod]
    public void Create_WithPastDate_ShouldThrowArgumentException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 10));

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = new DateTime(2026, 6, 1), // ← pasada respecto al clock
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        Assert.ThrowsException<ArgumentException>(() => service.CreateMatch(request));
    }
    
    [TestMethod]
    public void SetResult_WithValidData_ShouldUpdateMatchResult()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);
        
        var request = new MatchRequest
        {
            dateMatch = new DateTime(2026, 6, 1),
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        var created = service.CreateMatch(request);
        
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

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        var created = service.CreateMatch(request);

        Assert.ThrowsException<ArgumentException>(() => service.SetMatchResult(created.idMatch, -1, 0));
    }
    
    [TestMethod]
    public void SetResult_WhenMatchNotPlayedYet_ShouldThrowInvalidOperationException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();

        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);
        
        var request = new MatchRequest
        {
            dateMatch = new DateTime(2026, 6, 10),
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        var created = service.CreateMatch(request);
        
        Assert.ThrowsException<InvalidOperationException>(() => service.SetMatchResult(created.idMatch, 2, 1));
    }
    
    [TestMethod]
    public void SetResult_WhenResultAlreadySet_ShouldThrowInvalidOperationException()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        var created = service.CreateMatch(request);
        service.SetMatchResult(created.idMatch, 1, 0);
        Assert.ThrowsException<InvalidOperationException>(() => service.SetMatchResult(created.idMatch, 2, 0));
    }
    
    [TestMethod]
    public void GetById_WithExistingMatch_ShouldReturnMatchResponse()
    {
        var matchRepository = new MatchRepository();
        var teamRepository = new TeamRepositoryStub();
        var stadiumRepository = new StadiumRepositoryStub();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var request = new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };

        var created = service.CreateMatch(request);
        var result = service.GetMatchById(created.idMatch);
        Assert.AreEqual(created.idMatch, result.idMatch);
        Assert.AreEqual("Centenario", result.stadiumName);
        Assert.AreEqual("Uruguay", result.homeTeamName);
        Assert.AreEqual("Argentina", result.awayTeamName);
    }
    
    [TestMethod]
    public void GetById_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        var service = new MatchService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));
        Assert.ThrowsException<KeyNotFoundException>(() => service.GetMatchById(99));
    }
    
    [TestMethod]
    public void GetAll_WhenThereAreNoMatches_ShouldReturnEmptyList()
    {
        var service = new MatchService(new MatchRepository(), new TeamRepositoryStub(), new StadiumRepositoryStub(), new DateTimeMatchStub(new DateTime(2026, 6, 1)));
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

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        teamRepository.Add(new TeamStub("Brasil"));
        teamRepository.Add(new TeamStub("Peru"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        service.CreateMatch(new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        });

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

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        });

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

        teamRepository.Add(new TeamStub("Uruguay"));
        teamRepository.Add(new TeamStub("Argentina"));
        stadiumRepository.Add(new StadiumStub("Centenario"));

        var service = new MatchService(matchRepository, teamRepository, stadiumRepository, clock);

        var created = service.CreateMatch(new MatchRequest
        {
            dateMatch = clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        });

        var pastDate = new DateTime(2026, 6, 1);
        Assert.ThrowsException<ArgumentException>(() => service.UpdateMatchDate(created.idMatch, pastDate));
    }
}