using Domain.Entities;
using Domain.Interfaces;

namespace Services;

public class ProbabilisticSimulationEngine : ISimulationEngine
{
    private const int MaxGoals = 7;

    public string Name => "Probabilístico";

    public (int homeGoals, int awayGoals) SimulateMatch(int homeRanking, int awayRanking, int seed)
    {
        var random = new Random(seed);

        double homeStrength = 1.0 / (1.0 + Math.Pow(10, (double)(awayRanking - homeRanking) / 1000.0));
        
        double probability = random.NextDouble();

        int homeGoals;
        int awayGoals;

        if (probability < homeStrength)
        {
            homeGoals = random.Next(1, MaxGoals);
            awayGoals = random.Next(0, homeGoals);
        } else if (probability < homeStrength + (1 - homeStrength) / 2)
        {
            homeGoals = random.Next(0, MaxGoals);
            awayGoals = homeGoals;
        }
        else
        {
            awayGoals = random.Next(1, MaxGoals);
            homeGoals = random.Next(0, awayGoals);
        }
        return (homeGoals, awayGoals);
    }
}