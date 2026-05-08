using Domain;
using DTOs;
using IRepository;
using Repository;
using Services;
using Services.Mappers;

namespace ServicesTests;

[TestClass]
public class FixtureServiceTests
{
    private FixtureRepository _fixtureRepository;
    private FixtureService _service;
    private TournamentConfig _config;
    private Fixture _fixture;
    
    [TestInitialize]
    public void Setup()
    {
        _fixtureRepository = new FixtureRepository();
        _service = new FixtureService(_fixtureRepository);
        _config = new TournamentConfig();
        _fixture = new Fixture(_config);
        Match.ResetIdCounter();
    }
    
    [TestMethod]
    public void HasFixture_WhenNoFixture_ReturnsFalse()
    {
        var result = _service.HasFixture();
        Assert.IsFalse(result);
    }
}