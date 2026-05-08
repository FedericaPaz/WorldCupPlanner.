using Repository;
using Domain;

namespace RepositoryTests;

[TestClass]
public class FixtureRepositoryTests
{
    private FixtureRepository _repository;
    private Fixture _fixture;
    
    [TestInitialize]
    public void Setup()
    {
        _repository = new FixtureRepository();
        var config = new TournamentConfig();
        _fixture = new Fixture(config);
        _fixture.InitializeGroups();
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
        Assert.IsTrue(_repository.ContainsFixture(_fixture));
    }
    
    [TestMethod]
    public void GetFixture_AfterAdd_ReturnsSameFixture()
    {
        _repository.AddFixture(_fixture);
        var result = _repository.GetFixture();
        Assert.IsNotNull(result);
        Assert.AreSame(_fixture, result);
    }
    
    [TestMethod]
    public void GetFixture_WhenNoFixtureSaved_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _repository.GetFixture());
    }
    
    [TestMethod]
    public void ContainsFixture_WhenNoFixtureSaved_ReturnsFalse()
    {
        var fixture = _fixture;
        var result = _repository.ContainsFixture(fixture);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ContainsFixture_WhenFixtureSaved_ReturnsTrue()
    {
        var fixture = _fixture;
        _repository.AddFixture(fixture);
        var result = _repository.ContainsFixture(fixture);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveFixture_WhenFixtureSaved_RemovesFixture()
    {
        var fixture = _fixture;
        _repository.AddFixture(fixture);
        _repository.RemoveFixture(fixture);
        var result = _repository.ContainsFixture(fixture);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void RemoveFixture_WhenFixtureNotSaved_DoesNotThrowException()
    {
        var fixture = _fixture;
        _repository.RemoveFixture(fixture);
        Assert.IsFalse(_repository.ContainsFixture(_fixture));
    }
}