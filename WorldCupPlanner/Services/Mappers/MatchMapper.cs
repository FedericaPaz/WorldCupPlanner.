using Domain.Entities;
using Domain.Enums;
using DTOs.Match;

namespace Services.Mappers;

public static class MatchMapper
{
    public static MatchResponse ToResponse(this Match match)
    {
        return new MatchResponse
        {
            idMatch = match.IdMatch,
            dateMatch = match.DateMatch,
            stadiumName = match.GetStadiumName(),
            homeTeamName = match.GetHomeTeamName(),
            awayTeamName = match.GetAwayTeamName(),
            group = match.Group,
            homeGoals = match.HomeGoals,
            awayGoals = match.AwayGoals,
            winnerName = match.WinnerMatch?.Name,
            isLocked = match.IsLocked,
            phase = match.Group == 'N' ? "Eliminatoria" : "Grupos",
            homeYellowCards = GetIncidentQuantity(match, match.GetHomeTeamName(), IncidentType.YellowCard),
            awayYellowCards = GetIncidentQuantity(match, match.GetAwayTeamName(), IncidentType.YellowCard),
            homeRedCards = GetIncidentQuantity(match, match.GetHomeTeamName(), IncidentType.RedCard),
            awayRedCards = GetIncidentQuantity(match, match.GetAwayTeamName(), IncidentType.RedCard)
        };
    }

    private static int GetIncidentQuantity(Match match, string teamName, IncidentType type)
    {
        return match.Incidents
            .Where(incident =>
                incident.TeamName == teamName &&
                incident.Type == type)
            .Sum(incident => incident.Quantity);
    }
}