using Domain.Entities;
using Domain.Enums;
using DomainTests.Stubs;
using Repository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class RankingServiceTests
{
    private RankingService _service = null!;
    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private Team _strongTeam = null!;
    private Team _weakTeam = null!;
    private Match _match = null!;

    [TestInitialize]
    public void Setup()
    {
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var confederation = new Confederation("CONMEBOL");

        _strongTeam = new Team("Equipo Fuerte", confederation, 1500);
        _weakTeam = new Team("Equipo Debil", confederation, 1200);

        var stadium = new Stadium("Centenario", "Montevideo", null, 60000);
        _match = new Match(1, clock.Now, stadium, _strongTeam, _weakTeam, 'A', clock);

        _logRepository = new LogRepositoryStub();
        _logService = new LogService(_logRepository);
        _service = new RankingService(_logService);
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenWeakAwayTeamWins_ShouldIncreaseAwayTeamCurrentRanking()
    {
        _match.SetMatchResult(0, 1);

        _service.UpdateRankingsAfterMatch(_match);

        Assert.AreEqual(1220, _weakTeam.CurrentRanking);
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenWeakAwayTeamWins_ShouldDecreaseHomeTeamCurrentRanking()
    {
        _match.SetMatchResult(0, 1);

        _service.UpdateRankingsAfterMatch(_match);

        Assert.AreEqual(1480, _strongTeam.CurrentRanking);
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenMatchIsADraw_ShouldNotChangeRankingsOfEqualTeams()
    {
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var home = new Team("TeamA", new Confederation("UEFA"), 1500);
        var away = new Team("TeamB", new Confederation("UEFA"), 1500);
        var stadium = new Stadium("Estadio", "Ciudad", null, 50000);
        var match = new Match(2, clock.Now, stadium, home, away, 'A', clock);

        match.SetMatchResult(1, 1);

        _service.UpdateRankingsAfterMatch(match);

        Assert.AreEqual(1500, home.CurrentRanking);
        Assert.AreEqual(1500, away.CurrentRanking);
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenStrongTeamWins_ShouldChangeRankingsSlightly()
    {
        _match.SetMatchResult(1, 0);

        _service.UpdateRankingsAfterMatch(_match);

        Assert.IsTrue(_strongTeam.CurrentRanking > 1500, "El equipo fuerte que gana debería subir ligeramente");
        Assert.IsTrue(_weakTeam.CurrentRanking < 1200, "El equipo débil que pierde debería bajar ligeramente");
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_ShouldNotModifyFifaRanking()
    {
        _match.SetMatchResult(0, 1);

        _service.UpdateRankingsAfterMatch(_match);

        Assert.AreEqual(1500, _strongTeam.FifaRanking);
        Assert.AreEqual(1200, _weakTeam.FifaRanking);
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WithNullMatch_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.UpdateRankingsAfterMatch(null));
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenMatchHasNoResult_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.UpdateRankingsAfterMatch(_match));
    }

    [TestMethod]
    public void UpdateRankingsAfterMatch_WithValidMatch_ShouldRegisterRankingUpdatedLog()
    {
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var confederation = new Confederation("CONMEBOL");
        var strongTeam = new Team("Equipo Fuerte", confederation, 1500);
        var weakTeam = new Team("Equipo Debil", confederation, 1200);
        var stadium = new Stadium("Centenario", "Montevideo", "Estadio principal", 60000);
        var match = new Match(1, clock.Now, stadium, strongTeam, weakTeam, 'A', clock);

        var logRepository = new LogRepositoryStub();
        var logService = new LogService(logRepository);
        var service = new RankingService(logService);

        match.SetMatchResult(0, 1);

        service.UpdateRankingsAfterMatch(match);

        var logs = logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("system", logs[0].UserEmail);
        Assert.AreEqual("Ranking", logs[0].Category);
        Assert.AreEqual("RankingUpdated", logs[0].Action);
        StringAssert.Contains(logs[0].Detail, "Equipo Fuerte");
        StringAssert.Contains(logs[0].Detail, "Equipo Debil");
    }
}