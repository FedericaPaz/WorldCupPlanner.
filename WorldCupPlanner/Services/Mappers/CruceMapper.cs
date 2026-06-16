using Domain.Entities;
using DTOs.Cruces;

namespace Services.Mappers;

public static class CruceMapper
{
    public static CruceResponse ToResponse(Cruce cruce)
    {
        return new CruceResponse
        {
            Name = cruce.Name,
            HomeTeam = cruce.GetHomeTeamName(),
            AwayTeam = cruce.GetAwayTeamName(),
            HomeGoals = cruce.GetHomeGoals(),
            AwayGoals = cruce.GetAwayGoals(),
            Phase = (TournamentPhaseDTOs)cruce.Phase
        };
    }
}