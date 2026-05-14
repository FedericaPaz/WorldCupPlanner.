using Domain;
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
        Assert.AreEqual(_config.SemillaFixture, result.SemillaFixture);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaCompletar()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.SemillaCompletar, result.SemillaCompletar);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaCrucesFase()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.SemillaCrucesFase, result.SemillaCrucesFase);
    }

    [TestMethod]
    public void GetConfig_ReturnsSemillaSimulation()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.SemillaSimulation, result.SemillaSimulation);
    }

    [TestMethod]
    public void GetConfig_ReturnsFechaInicioTorneo()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.FechaInicioTorneo, result.FechaInicioTorneo);
    }

    [TestMethod]
    public void GetConfig_ReturnsMaxPartidosPorDia()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.MaxPartidosPorDia, result.MaxPartidosPorDia);
    }

    [TestMethod]
    public void GetConfig_ReturnsSeparacionEntreFechas()
    {
        var result = _service.GetConfig();
        Assert.AreEqual(_config.SeparacionEntreFechas, result.SeparacionEntreFechas);
    }

    [TestMethod]
    public void GetConfig_WithCustomConfig_ReturnsCorrectValues()
    {
        var customConfig = new TournamentConfig(
            semillaFixture: 111,
            semillaCompletar: 222,
            semillaCrucesFase: 333,
            semillaSimulation: 444,
            fechaInicioTorneo: new DateTime(2026, 7, 1),
            maxPartidosPorDia: 5,
            separacionEntreFechas: 4);

        var service = new ConfigService(customConfig);
        var result = service.GetConfig();

        Assert.AreEqual(111, result.SemillaFixture);
        Assert.AreEqual(222, result.SemillaCompletar);
        Assert.AreEqual(333, result.SemillaCrucesFase);
        Assert.AreEqual(444, result.SemillaSimulation);
        Assert.AreEqual(new DateTime(2026, 7, 1), result.FechaInicioTorneo);
        Assert.AreEqual(5, result.MaxPartidosPorDia);
        Assert.AreEqual(4, result.SeparacionEntreFechas);
    }
}