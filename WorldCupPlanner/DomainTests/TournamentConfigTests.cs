using Domain.Entities;

namespace DomainTests;

[TestClass]
public class TournamentConfigTests
{
    [TestMethod]
    public void Constructor_WithNoParameters_SetsDefaultValues()
    {
        var config = new TournamentConfig();
        Assert.AreEqual(12345, config.FixtureSeed);
        Assert.AreEqual(67890, config.FillingSeed);
        Assert.AreEqual(13579, config.PhaseDrawSeed);
        Assert.AreEqual(24680, config.SimulationSeed);
        Assert.AreEqual(new DateTime(2026, 6, 20), config.TournamentStartDate); //revisar
        Assert.AreEqual(3, config.MaxMatchesPerDay);
        Assert.AreEqual(3, config.DaysBetweenMatchdays);
    }

    [TestMethod]
    public void Constructor_WithParameters_SetsDefaultValues()
    {
        var customDate = new DateTime(2026, 6, 1);
        var config = new TournamentConfig(
            fixtureSeed: 99999,
            fillingSeed: 88888,
            phaseDrawSeed: 77777,
            simulationSeed: 66666,
            tournamentStartDate: customDate,
            maxMatchesPerDay: 4,
            daysBetweenMatchdays: 2
        );
        
        Assert.AreEqual(99999, config.FixtureSeed);
        Assert.AreEqual(88888, config.FillingSeed);
        Assert.AreEqual(77777, config.PhaseDrawSeed);
        Assert.AreEqual(66666, config.SimulationSeed);
        Assert.AreEqual(customDate, config.TournamentStartDate);
        Assert.AreEqual(4, config.MaxMatchesPerDay);
        Assert.AreEqual(2, config.DaysBetweenMatchdays);
    }
    
    [TestMethod]
    public void Constructor_WithNegativeSeed_ThrowsArgumentException()
    {
        var fecha = new DateTime(2026, 6, 1);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(-1, 1, 1, 1, fecha, 3, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(1, -1, 1, 1, fecha, 3, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(1, 1, -1, 1, fecha, 3, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(1, 1, 1, -1, fecha, 3, 3));
    }
    
    [TestMethod]
    public void Constructor_WithInvalidMaxPartidosPorDia_ThrowsArgumentException()
    {
        var  fecha = new DateTime(2026, 6, 1);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(1, 1, 1, 1, fecha, 0, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(1, 1, 1, 1, fecha, -3, 3));
    }
    
    [TestMethod]
    public void Constructor_WithInvalidSeparacionEntreFechas_ThrowsArgumentException()
    {
        var  fecha = new DateTime(2026, 6, 1);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TournamentConfig(1, 1, 1, 1, fecha, 3, -3));
    }
}