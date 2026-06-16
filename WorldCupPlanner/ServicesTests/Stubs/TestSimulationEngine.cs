using Domain.Interfaces;

namespace ServicesTests.Stubs;

public class TestSimulationEngine : ISimulationEngine
{
    public string Name { get; }

    public TestSimulationEngine(string name)
    {
        Name = name;
    }

    public (int homeGoals, int awayGoals) SimulateMatch(int homeRanking, int awayRanking, int seed)
    {
        // deterministic but simple function based on seed
        int home = (seed + homeRanking) % 5;
        int away = (seed + awayRanking) % 5;
        return (home, away);
    }
}
