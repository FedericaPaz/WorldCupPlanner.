using Services;

namespace ServicesTests;

[TestClass]
public class ProbabilisticSimulationEngineTests
{
    private ProbabilisticSimulationEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        _engine = new ProbabilisticSimulationEngine();
    }

    [TestMethod]
    public void Name_ShouldBeProbabilistico()
    {
        Assert.AreEqual("Probabilístico", _engine.Name);
    }

    [TestMethod]
    public void Simulate_SameSeedSameRankings_ReturnsSameResult()
    {
        var result1 = _engine.SimulateMatch(1500, 1200, 42);
        var result2 = _engine.SimulateMatch(1500, 1200, 42);

        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void Simulate_DifferentSeeds_ReturnsDifferentResults()
    {
        var result1 = _engine.SimulateMatch(1500, 1200, 42);
        var result2 = _engine.SimulateMatch(1500, 1200, 99);

        Assert.AreNotEqual(result1, result2);
    }

    [TestMethod]
    public void Simulate_GoalsAreWithinValidRange()
    {
        for (int seed = 0; seed < 100; seed++)
        {
            var (home, away) = _engine.SimulateMatch(1500, 1200, seed);

            Assert.IsTrue(home >= 0 && home <= 7, $"homeGoals={home} fuera de rango");
            Assert.IsTrue(away >= 0 && away <= 7, $"awayGoals={away} fuera de rango");
        }
    }

    [TestMethod]
    public void Simulate_StrongerTeamWinsMoreOften()
    {
        int strongerWins = 0;
        int totalMatches = 1000;

        for (int seed = 0; seed < totalMatches; seed++)
        {
            var (home, away) = _engine.SimulateMatch(2000, 500, seed);
            if (home > away) strongerWins++;
        }

        Assert.IsTrue(strongerWins > totalMatches * 0.6, $"El equipo más fuerte ganó solo {strongerWins}/{totalMatches} veces");
    }

    [TestMethod]
    public void Simulate_EqualRankings_ResultsAreVaried()
    {
        var results = new HashSet<(int, int)>();

        for (int seed = 0; seed < 50; seed++)
            results.Add(_engine.SimulateMatch(1000, 1000, seed));

        Assert.IsTrue(results.Count > 5, "Con rankings iguales debería haber variedad de resultados");
    }

    [TestMethod]
    public void Simulate_HomeTeamMuchStronger_HomeGoalsGreaterOnAverage()
    {
        int homeTotalGoals = 0;
        int awayTotalGoals = 0;
        int matches = 500;

        for (int seed = 0; seed < matches; seed++)
        {
            var (home, away) = _engine.SimulateMatch(2500, 300, seed);
            homeTotalGoals += home;
            awayTotalGoals += away;
        }

        Assert.IsTrue(homeTotalGoals > awayTotalGoals, $"Local ({homeTotalGoals}) debería tener más goles que visitante ({awayTotalGoals})");
    }
}