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
}