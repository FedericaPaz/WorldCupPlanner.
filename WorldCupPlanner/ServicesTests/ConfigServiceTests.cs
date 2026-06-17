using Domain.Entities;
using Services;

namespace ServicesTests;

[TestClass]
public class ConfigServiceTests
{
    private TournamentConfig _config;
    private ConfigService _service;

    [TestInitialize]
    public void Setup()
    {
        _config = new TournamentConfig();
        _service = new ConfigService(_config);
    }

    [TestMethod]
    public void CreateConfigService_WithNullConfig_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ConfigService(null));
    }

    [TestMethod]
    public void GetConfig_ReturnsNotNull()
    {
        var result = _service.GetConfig();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaFixture()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.FixtureSeed, result.FixtureSeed);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaCompletar()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.FillingSeed, result.FillingSeed);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaCrucesFase()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.PhaseDrawSeed, result.PhaseDrawSeed);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaSimulation()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.SimulationSeed, result.SimulationSeed);
    }

    [TestMethod]
    public void GetConfig_ReturnsFechaInicioTorneo()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.TournamentStartDate, result.TournamentStartDate);
    }

    [TestMethod]
    public void GetConfig_ReturnsMaxPartidosPorDia()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.MaxMatchesPerDay, result.MaxMatchesPerDay);
    }

    [TestMethod]
    public void GetConfig_ReturnsSeparacionEntreFechas()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.DaysBetweenMatchdays, result.DaysBetweenMatchdays);
    }

    [TestMethod]
    public void GetConfig_WithCustomConfig_ReturnsCorrectValues()
    {
        var customConfig = new TournamentConfig(
            fixtureSeed: 111,
            fillingSeed: 222,
            phaseDrawSeed: 333,
            simulationSeed: 444,
            tournamentStartDate: new DateTime(2026, 7, 1),
            maxMatchesPerDay: 5,
            daysBetweenMatchdays: 4);

        var service = new ConfigService(customConfig);
        var result = service.GetConfig();

        Assert.AreEqual(111, result.FixtureSeed);
        Assert.AreEqual(222, result.FillingSeed);
        Assert.AreEqual(333, result.PhaseDrawSeed);
        Assert.AreEqual(444, result.SimulationSeed);
        Assert.AreEqual(new DateTime(2026, 7, 1), result.TournamentStartDate);
        Assert.AreEqual(5, result.MaxMatchesPerDay);
        Assert.AreEqual(4, result.DaysBetweenMatchdays);
    }
}