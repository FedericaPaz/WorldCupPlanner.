using Services;

namespace ServicesTests;

[TestClass]
public class PureRandomSimulationEngineTests
{
    private PureRandomSimulationEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        _engine = new PureRandomSimulationEngine();
    }

    [TestMethod]
    public void Name_ShouldBeAleatorio()
    {
        Assert.AreEqual("Aleatorio puro", _engine.Name);
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
    public void Simulate_IgnoresRankings_SameSeedDifferentRankingsReturnsSameResult()
    {
        var result1 = _engine.SimulateMatch(2500, 300, 42);
        var result2 = _engine.SimulateMatch(300, 2500, 42);

        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void Simulate_ResultsAreVaried()
    {
        var results = new HashSet<(int, int)>();

        for (int seed = 0; seed < 50; seed++)
            results.Add(_engine.SimulateMatch(1000, 1000, seed));

        Assert.IsTrue(results.Count > 5, "Debería haber variedad de resultados con distintas semillas");
    }

    [TestMethod]
    public void Simulate_StrongerTeamDoesNotWinMoreOften()
    {
        int strongerWins = 0;
        int weakerWins = 0;
        int total = 1000;

        for (int seed = 0; seed < total; seed++)
        {
            var (home, away) = _engine.SimulateMatch(2500, 300, seed);
            if (home > away) strongerWins++;
            else if (away > home) weakerWins++;
        }

        double diff = Math.Abs(strongerWins - weakerWins) / (double)total;
        Assert.IsTrue(diff < 0.1, $"El motor aleatorio no debería favorecer a ningún equipo. Fuerte: {strongerWins}, Débil: {weakerWins}");
    }
}