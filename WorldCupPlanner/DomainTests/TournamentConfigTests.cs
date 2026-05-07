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
}