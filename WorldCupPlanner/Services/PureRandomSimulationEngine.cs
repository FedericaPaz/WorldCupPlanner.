using Domain.Entities;
using Domain.Interfaces;

namespace Services;

public class PureRandomSimulationEngine : ISimulationEngine
{
    private const int MaxGoals = 7;

    public string Name => "Aleatorio puro";

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
    {
        var random = new Random(seed + match.IdMatch);

        int homeGoals = random.Next(0, MaxGoals + 1);
        int awayGoals = random.Next(0, MaxGoals + 1);

        return (homeGoals, awayGoals);
    }
}