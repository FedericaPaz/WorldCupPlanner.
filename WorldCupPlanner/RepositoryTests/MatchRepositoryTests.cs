using Domain;
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
}