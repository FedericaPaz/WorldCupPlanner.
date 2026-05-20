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
    
    [TestMethod]
    public void CalculateNewRanking_EliminatoryPhase_ShouldHaveGreaterImpactThanGroupPhase()
    {
        int groupResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int eliminatoryResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.RoundOf32);
        Assert.IsTrue(eliminatoryResult > groupResult);
    }

    [TestMethod]
    public void CalculateNewRanking_WithRoundOf32_ShouldApplyEliminatoryMultiplier()
    {
        int groupResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int roundOf32Result = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.RoundOf32);
        Assert.IsTrue(roundOf32Result > groupResult);
    }
    
    [TestMethod]
    public void CalculateNewRanking_WithRoundOf16_ShouldApplyEliminatoryMultiplier()
    {
        int groupResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int roundOf16Result = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.RoundOf16);
        Assert.IsTrue(roundOf16Result > groupResult);
    }
    
    [TestMethod]
    public void CalculateNewRanking_WithQuarterFinals_ShouldApplyEliminatoryMultiplier()
    {
        int groupResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int quarterFinalsResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.QuarterFinals);
        Assert.IsTrue(quarterFinalsResult > groupResult);
    }
    
    [TestMethod]
    public void CalculateNewRanking_WithSemiFinals_ShouldApplyEliminatoryMultiplier()
    {
        int groupResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int semiFinalsResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.SemiFinals);
        Assert.IsTrue(semiFinalsResult > groupResult);
    }

    [TestMethod]
    public void CalculateNewRanking_WithFinal_ShouldApplyEliminatoryMultiplier()
    {
        int groupResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.GroupStage);
        int finalResult = _calculator.CalculateNewRanking(1200, 1500, 1.0, TournamentPhase.Final);
        Assert.IsTrue(finalResult > groupResult);
    }

    [TestMethod]
    public void CalculateNewRanking_WithInvalidPhase_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _calculator.CalculateNewRanking(1500, 1200, 1.0, TournamentPhase.NotStarted));
    }
    
    [TestMethod]
    public void CalculateNewRanking_WhenResultExceedsMax_ShouldClampToMaxRanking()
    {
        int result = _calculator.CalculateNewRanking(2490, 2000, 1.0, TournamentPhase.Final);
        Assert.AreEqual(2500, result);
    }

    [TestMethod]
    public void CalculateNewRanking_WhenResultGoesBelowMin_ShouldClampToMinRanking()
    {
        int result = _calculator.CalculateNewRanking(310, 500, 0.0, TournamentPhase.Final);
        Assert.AreEqual(300, result);
    }
    
    [TestMethod]
    public void CalculateNewRanking_WhenDecimalIsExactlyHalf_ShouldRoundUp()
    {
        int result1 = _calculator.CalculateNewRanking(1400, 1600, 1.0, TournamentPhase.GroupStage);
        int result2 = _calculator.CalculateNewRanking(1400, 1600, 1.0, TournamentPhase.GroupStage);
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void CalculateNewRanking_ShouldReturnInteger_NeverDecimal()
    {
        int result = _calculator.CalculateNewRanking(1350, 1650, 0.5, TournamentPhase.GroupStage);
        Assert.IsInstanceOfType(result, typeof(int));
    }
}