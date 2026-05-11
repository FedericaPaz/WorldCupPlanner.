using Domain;
using DTOs;

namespace Services.Mappers;

public static class CruceMapper
{
    public static CruceResponse ToResponse(Cruce cruce)
    {
        return new CruceResponse
        {
            Name = cruce.Nombre,
            HomeTeam = cruce.Partido.HomeTeam.Name,
            AwayTeam = cruce.Partido.AwayTeam.Name,
            HomeGoals = cruce.Partido.HomeGoals,
            AwayGoals = cruce.Partido.AwayGoals,
            Phase = cruce.Phase.ToString()
        };
    }
}