using Domain.Policies;
using Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests;

[TestClass]
public class PhaseTransitionPolicyTests
{
    private readonly DefaultPhaseTransitionPolicy _policy = new DefaultPhaseTransitionPolicy();

    [TestMethod]
    public void RoundOf32_To_RoundOf16()
    {
        Assert.AreEqual(TournamentPhase.RoundOf16, _policy.GetNextPhase(TournamentPhase.RoundOf32));
    }

    [TestMethod]
    public void RoundOf16_To_QuarterFinals()
    {
        Assert.AreEqual(TournamentPhase.QuarterFinals, _policy.GetNextPhase(TournamentPhase.RoundOf16));
    }

    [TestMethod]
    public void QuarterFinals_To_SemiFinals()
    {
        Assert.AreEqual(TournamentPhase.SemiFinals, _policy.GetNextPhase(TournamentPhase.QuarterFinals));
    }

    [TestMethod]
    public void SemiFinals_To_Final()
    {
        Assert.AreEqual(TournamentPhase.Final, _policy.GetNextPhase(TournamentPhase.SemiFinals));
    }

    [TestMethod]
    public void Final_ThrowsInvalidOperationException_WithSameMessage()
    {
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _policy.GetNextPhase(TournamentPhase.Final));
        Assert.AreEqual("La final no tiene siguiente fase", ex.Message);
    }

    [TestMethod]
    public void ThirdPlace_ThrowsInvalidOperationException_WithSameMessage()
    {
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _policy.GetNextPhase(TournamentPhase.ThirdPlace));
        Assert.AreEqual("El partido por el tercer puesto no tiene siguiente fase", ex.Message);
    }

    [TestMethod]
    public void GroupStage_ThrowsInvalidOperationException_WithDefaultMessage()
    {
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _policy.GetNextPhase(TournamentPhase.GroupStage));
        Assert.AreEqual($"La fase {TournamentPhase.GroupStage} no tiene siguiente definida", ex.Message);
    }
}
