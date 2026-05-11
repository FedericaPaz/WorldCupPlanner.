using IServices;
using Domain;

namespace Services;

public class CruceSimulationService : ISimulationService
{
    private const int MaxGoals = 7;

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
    {
        var home = (Team)match.HomeTeam;
        var away = (Team)match.AwayTeam;
        var random  = new Random(seed + match.IdMatch);
        double homeStrength = (double)home.FifaRanking / (home.FifaRanking + away.FifaRanking);
        int homeGoals = (int)Math.Round(homeStrength * random.Next(0, MaxGoals + 1));
        int awayGoals = (int)Math.Round((1 - homeStrength) * random.Next(0, MaxGoals + 1));
        return (homeGoals, awayGoals);
    }

    public void SimulateGroupStage(List<Group> groups, int seed)
    {
        throw new NotImplementedException();
    }

    public void SimulateCruces(List<Cruce> cruces, int seed)
    {
        throw new NotImplementedException();
    }
}