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
}