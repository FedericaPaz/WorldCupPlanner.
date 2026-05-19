using Domain.Entities;
using DomainTests.Stubs;
using Repository;

namespace RepositoryTests;

[TestClass]
public class MatchRepositoryTests
{
    [TestInitialize]
    public void Setup()
    {
        Match.ResetIdCounter();
    }

    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldStoreMatch()
    {
        var repository = new MatchRepository();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var result = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);

        repository.AddMatch(result);
        Assert.AreEqual(1, repository.GetAllMatches().Count);
        Assert.AreEqual(result, repository.GetAllMatches()[0]);
    }
    
    [TestMethod]
    public void GetById_WithExistingMatch_ShouldReturnMatch()
    {
        var repository = new MatchRepository();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);

        repository.AddMatch(match);
        var result = repository.GetMatchById(match.IdMatch);
        Assert.AreEqual(match, result);
    }
    
    [TestMethod]
    public void GetById_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        var repository = new MatchRepository();
        Assert.ThrowsException<KeyNotFoundException>(() => repository.GetMatchById(99));
    }
    
    [TestMethod]
    public void GetById_WithInvalidId_ShouldThrowArgumentException()
    {
        var repository = new MatchRepository();
        Assert.ThrowsException<ArgumentException>(() => repository.GetMatchById(0));
    }
    
    [TestMethod]
    public void GetById_WithNegativeId_ShouldThrowArgumentException()
    {
        var repository = new MatchRepository();
        Assert.ThrowsException<ArgumentException>(() => repository.GetMatchById(-1));
    }
    
    [TestMethod]
    public void GetAll_WhenRepositoryIsEmpty_ShouldReturnEmptyList()
    {
        var repository = new MatchRepository();
        var matches = repository.GetAllMatches();
        Assert.AreEqual(0, matches.Count);
    }
    
    [TestMethod]
    public void GetAll_WithMatches_ShouldReturnAllMatches()
    {
        var repository = new MatchRepository();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var match1 = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
        var match2 = new Match(clock.Now, new StadiumStub("Maracana"), new TeamStub("Brasil"), new TeamStub("Peru"), 'B', clock);
        repository.AddMatch(match1);
        repository.AddMatch(match2);
        var matches = repository.GetAllMatches();
        Assert.AreEqual(2, matches.Count);
        Assert.AreEqual(match1, matches[0]);
        Assert.AreEqual(match2, matches[1]);
    }
    
    [TestMethod]
    public void Add_WithNullMatch_ShouldThrowArgumentNullException()
    {
        var repository = new MatchRepository();
        Assert.ThrowsException<ArgumentNullException>(() => repository.AddMatch(null));
    }
}