using Domain.Enums;
using Domain.Calculators;
namespace DomainTests;

[TestClass]
public class RankingCalculatorTests
{
    private RankingCalculator _calculator = null!;

    [TestInitialize]
    public void SetUp()
    {
        _calculator = new RankingCalculator();
    }
    
    [TestMethod]
    public void CalculateNewRanking_WeakerTeamWins_ShouldIncreaseRanking()
    {
        int result = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        Assert.AreEqual(1220, result);
    }
    
    [TestMethod]
    public void CalculateNewRanking_StrongerTeamWins_ShouldIncreaseRankingSlightly()
    {
        int result = _calculator.CalculateNewRanking(1500, 1200, 1.0, TournamentPhase.GroupStage);
        Assert.AreEqual(1510, result);
    }

    [TestMethod]
    public void CalculateNewRanking_WithDraw_ShouldReturnExpectedRanking()
    {
        int result = _calculator.CalculateNewRanking(1500, 1500, 0.5, TournamentPhase.GroupStage);
        Assert.AreEqual(1500, result);
    }
}