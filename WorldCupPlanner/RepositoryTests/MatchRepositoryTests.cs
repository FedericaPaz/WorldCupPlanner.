using Domain.Entities;
using DomainTests.Stubs;
using Repository;
using Microsoft.EntityFrameworkCore;

namespace RepositoryTests;

[TestClass]
public class MatchRepositoryTests
{
    private WorldCupPlannerContext _context = null!;
    private MatchRepository _repository = null!;
    private DateTimeMatchStub _clock = null!;
    private int _nextId;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);

        _clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        _repository = new MatchRepository(_context, _clock);  
        _nextId = 1;
    }

    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("CONMEBOL"), 1500);
    }

    private Stadium CreateStadium(string name)
    {
        return new Stadium(name, "Montevideo", "", 60000);
    }

    private Match CreateMatch(string home = "Uruguay", string away = "Argentina",
        string stadium = "Centenario", char group = 'A')
    {
        return new Match(
            _nextId++,
            _clock.Now,
            CreateStadium(stadium),
            CreateTeam(home),
            CreateTeam(away),
            group,
            _clock);
    }

    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldStoreMatch()
    {
        var match = CreateMatch();
        SaveRelatedEntities(match);

        _repository.AddMatch(match);

        Assert.AreEqual(1, _repository.GetAllMatches().Count);
    }

    [TestMethod]
    public void GetById_WithExistingMatch_ShouldReturnMatch()
    {
        var match = CreateMatch();
        SaveRelatedEntities(match);
        
        _repository.AddMatch(match);

        var result = _repository.GetMatchById(match.IdMatch);

        Assert.AreEqual(match, result);
    }

    [TestMethod]
    public void GetById_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        Assert.ThrowsException<KeyNotFoundException>(() =>
            _repository.GetMatchById(99));
    }

    [TestMethod]
    public void GetById_WithInvalidId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _repository.GetMatchById(0));
    }

    [TestMethod]
    public void GetById_WithNegativeId_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _repository.GetMatchById(-1));
    }

    [TestMethod]
    public void GetAll_WhenRepositoryIsEmpty_ShouldReturnEmptyList()
    {
        Assert.AreEqual(0, _repository.GetAllMatches().Count);
    }

    [TestMethod]
    public void GetAll_WithMatches_ShouldReturnAllMatches()
    {
        var match1 = CreateMatch("Uruguay", "Argentina", "Centenario", 'A');
        var match2 = CreateMatch("Brasil", "Peru", "Maracana", 'B');
        SaveRelatedEntities(match1);
        SaveRelatedEntities(match2);
        
        _repository.AddMatch(match1);
        _repository.AddMatch(match2);

        var matches = _repository.GetAllMatches();

        Assert.AreEqual(2, matches.Count);
        Assert.AreEqual(match1.IdMatch, matches[0].IdMatch);
        Assert.AreEqual(match2.IdMatch, matches[1].IdMatch);
    }

    [TestMethod]
    public void Add_WithNullMatch_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _repository.AddMatch(null!));
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
    
    private void SaveRelatedEntities(Match match)
    {
        if (!_context.Teams.Any(t => t.Name == match.HomeTeam.Name))
            _context.Teams.Add(match.HomeTeam);
        if (!_context.Teams.Any(t => t.Name == match.AwayTeam.Name))
            _context.Teams.Add(match.AwayTeam);
        if (!_context.Stadiums.Any(s => s.StadiumName == match.Stadium.StadiumName))
            _context.Stadiums.Add(match.Stadium);
        _context.SaveChanges();
    }
}