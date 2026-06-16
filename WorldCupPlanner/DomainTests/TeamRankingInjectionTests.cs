using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace DomainTests;

[TestClass]
public class TeamRankingInjectionTests
{
    private class StubRankingCalculator : IRankingCalculator
    {
        public int CalledWithCurrent { get; private set; }
        public int CalledWithOpponent { get; private set; }
        public double CalledWithResult { get; private set; }
        public TournamentPhase CalledWithPhase { get; private set; }

        private readonly int _returnValue;

        public StubRankingCalculator(int returnValue)
        {
            _returnValue = returnValue;
        }

        public int CalculateNewRanking(int currentRanking, int opponentRanking, double result, TournamentPhase tournamentPhase)
        {
            CalledWithCurrent = currentRanking;
            CalledWithOpponent = opponentRanking;
            CalledWithResult = result;
            CalledWithPhase = tournamentPhase;
            return _returnValue;
        }
    }

    [TestMethod]
    public void RecalcularRanking_UsesInjectedCalculator()
    {
        var conf = new Confederation("AFC");
        var stub = new StubRankingCalculator(1700);
        var team = new Team("Test", conf, 1500, string.Empty, stub);

        team.RecalcularRanking(1400, 1.0, TournamentPhase.GroupStage);

        Assert.AreEqual(1700, team.CurrentRanking);
        // also verify stub received the expected parameters
        Assert.AreEqual(1500, stub.CalledWithCurrent);
        Assert.AreEqual(1400, stub.CalledWithOpponent);
        Assert.AreEqual(1.0, stub.CalledWithResult);
        Assert.AreEqual(TournamentPhase.GroupStage, stub.CalledWithPhase);
    }
}
