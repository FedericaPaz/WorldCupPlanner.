using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Match;

namespace Services.Mappers;

public static class EntityMatchMapper
{
    public static MatchResponse ToResponse(this Match match)
    {
        return new MatchResponse
        {
            idMatch = match.IdMatch,
            dateMatch = match.DateMatch,
            stadiumName = match.Stadium.StadiumName,
            homeTeamName = match.HomeTeam.Name,
            awayTeamName = match.AwayTeam.Name,
            group = match.Group,
            homeGoals = match.HomeGoals,
            awayGoals = match.AwayGoals,
            winnerName = match.WinnerMatch?.Name,
            isLocked     = match.IsLocked,
            phase        = match.Group == 'N' ? "Eliminatoria" : "Grupos",
            homeYellowCards = GetIncidentQuantity(match, match.HomeTeam, IncidentType.YellowCard),
            awayYellowCards = GetIncidentQuantity(match, match.AwayTeam, IncidentType.YellowCard),
            homeRedCards = GetIncidentQuantity(match, match.HomeTeam, IncidentType.RedCard),
            awayRedCards = GetIncidentQuantity(match, match.AwayTeam, IncidentType.RedCard)
        };
    }
    
    private static int GetIncidentQuantity(Match match, ITeam team, IncidentType type)
    {
        return match.Incidents
            .Where(incident => incident.Team == team && incident.Type == type)
            .Sum(incident => incident.Quantity);
    }
}