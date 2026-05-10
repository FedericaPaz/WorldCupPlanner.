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
        Assert.AreEqual(fase, cruce.Phase);
    }

    [TestMethod]
    public void Constructor_WithEmptyName_ThrowsException()
    {
        var partido = CreateBasicMatch();
        Assert.ThrowsException<ArgumentException>(() => new Cruce("", partido, TournamentPhase.RoundOf16));
    }
    
    [TestMethod]
    public void Constructor_WithNullName_ThrowsException()
    {
        var partido = CreateBasicMatch();
        Assert.ThrowsException<ArgumentException>(() => new Cruce(null, partido, TournamentPhase.RoundOf16));
    }

    [TestMethod]
    public void Constructor_WithNullPartido_ThrowsException()
    {
        var partido = CreateBasicMatch();
        Assert.ThrowsException<ArgumentNullException>((() => new Cruce("A1", null, TournamentPhase.RoundOf16)));
    }

    [TestMethod]
    public void DecideNextCruce_WithValidCruce_SetsNextPhase()
    {
        var partido1 = CreateBasicMatch();
        var partido2 = CreateBasicMatch();
        
        var cruce1 = new Cruce("A1", partido1, TournamentPhase.RoundOf16);
        var cruce2 = new Cruce("A2", partido2, TournamentPhase.QuarterFinals);
        
        cruce1.DecideNextCruce(cruce2);
        Assert.AreEqual(cruce2, cruce1.NextCruce);
    }
    
    [TestMethod]
    public void DecideNextCruce_WithSelfReference_ThrowsException()
    {
        var partido = CreateBasicMatch();
        var cruce = new Cruce("A1", partido, TournamentPhase.RoundOf16);
        Assert.ThrowsException<InvalidOperationException>(() => cruce.DecideNextCruce(cruce));
    }

    [TestMethod]
    public void GetWinner_WhenMatchHasResult_ReturnsWinner()
    {
        _homeTeam = new Team("Test homeTeam", new Confederation("UEFA"), 301);
        _awayTeam = new Team("Test awayTeam", new Confederation("UEFA"), 302);
        _stadium = new Stadium("Test Stadium", "Ciudad","Desc",50000);
        var match = CreateBasicMatch();
        match.SetMatchResult(2, 1);
        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf16);
        var winner = cruce.GetWinner();
        Assert.AreEqual(_homeTeam, winner);
    }
    
    [TestMethod]
    public void GetWinner_WhenMatchResultDraw_ReturnsNull()
    {
        _homeTeam = new Team("Test homeTeam", new Confederation("UEFA"), 301);
        _awayTeam = new Team("Test awayTeam", new Confederation("UEFA"), 302);
        _stadium = new Stadium("Test Stadium", "Ciudad","Desc",50000);
        var match = CreateBasicMatch();
        match.SetMatchResult(1, 1);
        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf16);
        var winner = cruce.GetWinner();
        Assert.IsNull(winner);
    }
    
    [TestMethod]
    public void GetWinner_WhenMatchResultNotSet_ReturnsNull()
    {
        var match = CreateBasicMatch();
        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf16);
        var winner = cruce.GetWinner();
        Assert.IsNull(winner);
    }
    
    [TestMethod]
    public void DecideNextCruce_WithValidCruce_SetsNextCruce()
    {
        var partido1 = CreateBasicMatch();
        var partido2 = CreateBasicMatch();
        
        var cruce1 = new Cruce("A1", partido1, TournamentPhase.RoundOf16);
        var cruce2 = new Cruce("A2", partido2, TournamentPhase.QuarterFinals);
        
        cruce1.DecideNextCruce(cruce2);
        Assert.AreEqual(cruce2, cruce1.NextCruce);
    }
    
    [TestMethod]
    public void DecideNextCruce_WithInvalidCruce_SetsNextCruce()
    {
        var partido1 = CreateBasicMatch();
        var partido2 = CreateBasicMatch();
        
        var cruce1 = new Cruce("A1", partido1, TournamentPhase.RoundOf16);
        var cruce2 = new Cruce("A2", partido2, TournamentPhase.SemiFinals);
        
        Assert.ThrowsException<InvalidOperationException>(() => cruce1.DecideNextCruce(cruce2));
    }
    
    
}