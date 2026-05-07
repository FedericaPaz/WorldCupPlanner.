using Domain;

namespace DomainTests;

[TestClass]
public class TournamentConfigTests
{
    [TestMethod]
    public void Constructor_WithNoParameters_SetsDefaultValues()
    {
        var config = new TournamentConfig();
        Assert.AreEqual(12345, config.SemillaFixture);
        Assert.AreEqual(67890, config.SemillaCompletar);
        Assert.AreEqual(13579, config.SemillaCrucesFase);
        Assert.AreEqual(24680, config.SemillaSimulation);
        Assert.AreEqual(new DateTime(2026, 6, 1), config.FechaInicioTorneo);
        Assert.AreEqual(3, config.MaxPartidosPorDia);
        Assert.AreEqual(3, config.SeparacionEntreFechas);
    }

    [TestMethod]
    public void Constructor_WithParameters_SetsDefaultValues()
    {
        var customDate = new DateTime(2026, 6, 1);
        var config = new TournamentConfig(
            semillaFixture: 99999,
            semillaCompletar: 88888,
            semillaCrucesFase: 77777,
            semillaSimulation: 66666,
            fechaInicioTorneo: customDate,
            maxPartidosPorDia: 4,
            separacionEntreFechas: 2
        );
        
        Assert.AreEqual(99999, config.SemillaFixture);
        Assert.AreEqual(88888, config.SemillaCompletar);
        Assert.AreEqual(77777, config.SemillaCrucesFase);
        Assert.AreEqual(66666, config.SemillaSimulation);
        Assert.AreEqual(customDate, config.FechaInicioTorneo);
        Assert.AreEqual(4, config.MaxPartidosPorDia);
        Assert.AreEqual(2, config.SeparacionEntreFechas);
    }
}