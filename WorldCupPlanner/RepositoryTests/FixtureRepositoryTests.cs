using Microsoft.EntityFrameworkCore;
using Repository;
using Domain.Entities;

namespace RepositoryTests;

[TestClass]
public class FixtureRepositoryTests
{
    private WorldCupPlannerContext _context;
    private FixtureRepository _repository;
    private Fixture _fixture;
    
    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);
        _repository = new FixtureRepository(_context);
        
        var config = new TournamentConfig();
        _fixture = new Fixture(config);
        _fixture.InitializeGroups();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    
    [TestMethod]
    public void AddFixture_WithNullFixture_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _repository.AddFixture(null));
    }

    [TestMethod]
    public void AddFixture_WithValidFixture_DoesNotThrowException()
    {
        _repository.AddFixture(_fixture);
        Assert.IsTrue(_repository.ContainsFixture());
    }
    
    [TestMethod]
    public void GetFixture_AfterAdd_ReturnsSameFixture()
    {
        _repository.AddFixture(_fixture);
        var result = _repository.GetFixture();
        Assert.IsNotNull(result);
        Assert.AreEqual(_fixture.Id, result.Id);
    }
    
    [TestMethod]
    public void GetFixture_WhenNoFixtureSaved_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _repository.GetFixture());
    }
    
    [TestMethod]
    public void ContainsFixture_WhenNoFixtureSaved_ReturnsFalse()
    {
        var result = _repository.ContainsFixture();
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ContainsFixture_WhenFixtureSaved_ReturnsTrue()
    {
        var fixture = _fixture;
        _repository.AddFixture(fixture);
        var result = _repository.ContainsFixture();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveFixture_WhenFixtureSaved_RemovesFixture()
    {
        var fixture = _fixture;
        _repository.AddFixture(fixture);
        _repository.RemoveFixture(fixture);
        var result = _repository.ContainsFixture();
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void RemoveFixture_WhenFixtureNotSaved_DoesNotThrowException()
    {
        var fixture = _fixture;
        _repository.RemoveFixture(fixture);
        Assert.IsFalse(_repository.ContainsFixture());
    }
    
    [TestMethod]
    public void AddFixture_OverwritesPreviousFixture()
    {
        var config1 = new TournamentConfig();
        var fixture1 = new Fixture(config1);
        fixture1.InitializeGroups();
        
        var config2 = new TournamentConfig();
        var fixture2 = new Fixture(config2);
        fixture2.InitializeGroups();
        
        _repository.AddFixture(fixture1);
        _repository.AddFixture(fixture2);
        
        var result = _repository.GetFixture();
        Assert.AreEqual(fixture2.Id, result.Id);
        Assert.AreNotEqual(fixture1.Id, result.Id);
    }
}
