using Domain.Entities;
using DomainTests.Stubs;
using Services;

namespace ServicesTests;

[TestClass]
public class RankingServiceTests
{
    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenWeakAwayTeamWins_ShouldIncreaseAwayTeamRanking()
    {
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var confederation = new Confederation("CONMEBOL");
        var strongTeam = new Team("Equipo Fuerte", confederation, 1500);
        var weakTeam = new Team("Equipo Debil", confederation, 1200);
        var stadium = new Stadium("Centenario", "Montevideo", null, 60000);
        var match = new Match(clock.Now, stadium, strongTeam, weakTeam, 'A', clock);
        var service = new RankingService();

        match.SetMatchResult(0, 1);

        service.UpdateRankingsAfterMatch(match);

        Assert.AreEqual(1220, weakTeam.FifaRanking);
    }
    
    [TestMethod]
    public void UpdateRankingsAfterMatch_WhenWeakAwayTeamWins_ShouldDecreaseHomeTeamRanking()
    {
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var confederation = new Confederation("CONMEBOL");
        var strongTeam = new Team("Equipo Fuerte", confederation, 1500);
        var weakTeam = new Team("Equipo Debil", confederation, 1200);
        var stadium = new Stadium("Centenario", "Montevideo", null, 60000);
        var match = new Match(clock.Now, stadium, strongTeam, weakTeam, 'A', clock);
        var service = new RankingService();

        match.SetMatchResult(0, 1);

        service.UpdateRankingsAfterMatch(match);

        Assert.AreEqual(1480, strongTeam.FifaRanking);
    }
}