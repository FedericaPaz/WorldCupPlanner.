using Domain;
using DTOs;

namespace Services.Mappers;

public class ConfederationXMappers
{
    public static ConfederationDTO FromEntity(Confederation confederation)
    {
        return new ConfederationDTO
        {
            Name = confederation.Name,
            MaxTeams = confederation.MaxTeams
        };
    }

    public static Confederation ToEntity(ConfederationDTO confederationDTO)
    {
        return new Confederation(confederationDTO.Name);
    }
}