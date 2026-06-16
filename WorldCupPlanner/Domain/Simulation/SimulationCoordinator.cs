using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Simulation;

public class SimulationCoordinator
{
    private const int MaxYellowCards = 4;
    private const int MaxRedCards = 1;

    public (int homeGoals, int awayGoals) SimulateAndApply(Match match, int seed, ISimulationEngine engine, bool resolverEmpates)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));
        if (engine == null) throw new ArgumentNullException(nameof(engine));

        var matchSeed = seed + match.IdMatch;
        var (homeGoals, awayGoals) = engine.SimulateMatch(match.HomeTeam.CurrentRanking, match.AwayTeam.CurrentRanking, matchSeed);

        if (resolverEmpates && homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5) homeGoals++;
            else awayGoals++;
        }

        // Clear and add simulated incidents using deterministic random based on seed and match id
        match.ClearCardIncidents();
        var random = new Random(seed + match.IdMatch * 17);

        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.HomeTeam, random.Next(0, MaxYellowCards + 1));
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.AwayTeam, random.Next(0, MaxYellowCards + 1));
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.HomeTeam, random.Next(0, MaxRedCards + 1));
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.AwayTeam, random.Next(0, MaxRedCards + 1));

        match.SimulateResult(homeGoals, awayGoals);
        return (homeGoals, awayGoals);
    }

    private static void AddIncidentIfNeeded(Match match, IncidentType type, Team team, int quantity)
    {
        if (quantity > 0)
            match.AddIncident(new MatchIncident(type, team, quantity));
    }
}
