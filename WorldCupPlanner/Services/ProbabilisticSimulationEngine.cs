using Domain.Entities;
using Domain.Interfaces;

namespace Services;

public class ProbabilisticSimulationEngine : ISimulationEngine
{
    private const int MaxGoals = 7;

    public string Name => "Probabilístico";

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
    {
        var home = match.HomeTeam;
        var away = match.AwayTeam;
        var random = new Random(seed + match.IdMatch);

        double homeStrength = (double)home.FifaRanking / (home.FifaRanking + away.FifaRanking);

        int homeGoals = (int)Math.Round(homeStrength * random.Next(0, MaxGoals + 1));
        int awayGoals = (int)Math.Round((1 - homeStrength) * random.Next(0, MaxGoals + 1));

        return (homeGoals, awayGoals);
    }
}