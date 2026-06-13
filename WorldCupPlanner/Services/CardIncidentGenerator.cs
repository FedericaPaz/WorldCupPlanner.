using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Services;

public static class CardIncidentGenerator
{
    private const int MaxYellowCards = 4;
    private const int MaxRedCards = 1;

    public static void Generate(Match match, int seed)
    {
        // Preserve existing behavior: clear previous card incidents and add simulated ones
        match.ClearCardIncidents();
        var random = new Random(seed + match.IdMatch * 17);

        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.HomeTeam, random.Next(0, MaxYellowCards + 1));
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.AwayTeam, random.Next(0, MaxYellowCards + 1));
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.HomeTeam, random.Next(0, MaxRedCards + 1));
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.AwayTeam, random.Next(0, MaxRedCards + 1));
    }

    private static void AddIncidentIfNeeded(Match match, IncidentType type, ITeam team, int quantity)
    {
        if (quantity > 0)
            match.AddIncident(new MatchIncident(type, (Team)team, quantity));
    }
}
