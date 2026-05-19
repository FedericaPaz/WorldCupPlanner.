using Domain.Entities;
using DTOs.Fixture;

namespace Services.Mappers;

public class StandingMapper
{
    public static StandingResponse ToResponse(Standing standing)
    {
        return new StandingResponse
        {
            TeamName = standing.TeamInGroup.Name,
            Points = standing.PointsInGroup,
            GoalsFor = standing.GoalsFor,
            GoalsAgainst = standing.GoalsAgainst,
            GoalDifference = standing.GoalDifference,
            MatchesPlayed = standing.MatchesPlayed,
            Wins = standing.Wins,
            Draws = standing.Draws,
            Losses = standing.Losses
        };
    }
}