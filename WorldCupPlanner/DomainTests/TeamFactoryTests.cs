using Domain.Entities;
using Domain.Import;
using Domain.Interfaces;

namespace DomainTests;

internal class RankingCalculatorStub : IRankingCalculator
{
    public int CalculateNewRanking(int currentRanking, int opponentRanking, double result, Domain.Enums.TournamentPhase phase)
    {
        return currentRanking;
    }
}

[TestClass]
public class TeamFactoryTests
{
    private IRankingCalculator _calculatorStub = null!;

    [TestInitialize]
    public void Initialize()
    {
        _calculatorStub = new RankingCalculatorStub();
    }

    [TestMethod]
    public void CreateTeam_WithValidData_ShouldCreateTeam()
    {
        var factory = new TeamFactory(_calculatorStub);
        
        Team team = factory.CreateTeam("Uruguay", "CONMEBOL", "1800", "base64-uruguay-flag");
        
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual("CONMEBOL", team.Confederation.Name);
        Assert.AreEqual(1800, team.FifaRanking);
        Assert.AreEqual("base64-uruguay-flag", team.FlagBase64);
    }

    [TestMethod]
    public void CreateTeam_WithNonNumericRanking_ShouldThrowException()
    {
        var factory = new TeamFactory(_calculatorStub);
        Assert.ThrowsException<ArgumentException>(() => factory.CreateTeam("Uruguay", "CONMEBOL", "A", "")); 
    }
}