using IServices;
using Domain;

namespace Services;

public class CrucesSimulationService : ISimulationService
{
    private const int MaxGoals = 7;
    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
    {
        var home = (Team)match.HomeTeam;
        var away = (Team)match.AwayTeam;
        var random = new Random(seed + match.IdMatch);
        double homeStrength = (double)home.FifaRanking / (home.FifaRanking + away.FifaRanking);
        int homeGoals = (int)Math.Round(homeStrength * random.Next(0, MaxGoals + 1));
        int awayGoals = (int)Math.Round((1 - homeStrength) * random.Next(0, MaxGoals + 1));
        return (homeGoals, awayGoals);
    }

    public void SimulateGroupStage(List<Group> groups, int seed)
    {
        foreach (var group in groups)
        foreach (var match in group.MatchesInGroup.Where(m => !m.HomeGoals.HasValue))
            SimulateAndSetResult(match, seed);
    }

    public void SimulateCruces(List<Cruce> cruces, int seed)
    {
        foreach (var cruce in cruces.Where(c => c.GetWinner() == null))
            SimulateAndSetResult(cruce.Match, seed);
    }

    private void SimulateAndSetResult(Match match, int seed)
    {
        var (homeGoals, awayGoals) = SimulateMatch(match, seed);
        if (homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5)
                homeGoals++;
            else
                awayGoals++;
        }
        match.SimulateResult(homeGoals, awayGoals);
    }
}