using Domain;
using DTOs;

namespace Services.Mappers;

public static class CruceMapper
{
    public static CruceResponse ToResponse(Cruce cruce)
    {
        return new CruceResponse
        {
            Name = cruce.Name,
            HomeTeam = cruce.Match.HomeTeam.Name,
            AwayTeam = cruce.Match.AwayTeam.Name,
            HomeGoals = cruce.Match.HomeGoals,
            AwayGoals = cruce.Match.AwayGoals,
            Phase = (TournamentPhaseDTOs)cruce.Phase
        };
    }
}