
using Domain;
using DTOs;

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
            winnerName = match.WinnerMatch?.Name
        };
    }
}