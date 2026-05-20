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
    
    [TestMethod]
    public void CalculateNewRanking_WithLoss_ShouldDecreaseRanking()
    {
        int result = _calculator.CalculateNewRanking(1500, 1200, 0.0, TournamentPhase.GroupStage);
        Assert.IsTrue(result < 1500);
    }

    [TestMethod]
    public void CalculateNewRanking_SymmetricTeams_DrawShouldNotChangeRanking()
    {
        int result = _calculator.CalculateNewRanking(1500, 1500, 0.5, TournamentPhase.GroupStage);
        Assert.AreEqual(1500, result);
    }

    [TestMethod]
    public void CalculateNewRanking_WinnerGainsWhatLoserLoses_InGroupStage()
    {
        int winnerNew = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int loserNew  = _calculator.CalculateNewRanking(1500, 1200, 0.0, TournamentPhase.GroupStage);
        int winnerGain = winnerNew - 1200;
        int loserLoss  = 1500 - loserNew;
        Assert.IsTrue(Math.Abs(winnerGain - loserLoss) <= 1);
    }
    
}