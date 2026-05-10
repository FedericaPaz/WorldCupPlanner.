using Domain;

namespace DomainTests;

[TestClass]
public class CrucesTests
{
    private IStadium _stadium;
    private ITeam _homeTeam;
    private ITeam _awayTeam;
    private IDateTimeMatch _clock;
    
    private Match CreateBasicMatch()
    {
        DateTime now = DateTime.Now;
        _stadium = new Stadium("Test Stadium", "Ciudad","Desc",50000);
        _homeTeam = new Team("Test homeTeam", new Confederation("UEFA"), 301);
        _awayTeam = new Team("Test awayTeam", new Confederation("UEFA"), 302);
        _clock = new ClockStub(now);
        var basicMatch = new Match(now, _stadium, _homeTeam, _awayTeam,'A', _clock);
        return basicMatch;
    }
    
    [TestMethod]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        var partido = CreateBasicMatch();
        string nombre = "A1";
        var fase = TournamentPhase.RoundOf16;
        var cruce = new Cruce(nombre, partido, fase);
        Assert.AreEqual(nombre, cruce.Nombre);
        Assert.AreEqual(partido, cruce.Partido);
        Assert.AreEqual(fase, cruce.Fase);
    }
}