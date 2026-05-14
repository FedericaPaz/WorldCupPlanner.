using Domain;
using DTOs;

namespace Services.Mappers;

public static class ConfigMapper
{
    public static TournamentConfigDTO ToDTO(TournamentConfig config)
    {
        return new TournamentConfigDTO
        {
            SemillaFixture = config.SemillaFixture,
            SemillaCompletar = config.SemillaCompletar,
            SemillaCrucesFase = config.SemillaCrucesFase,
            SemillaSimulation = config.SemillaSimulation,
            FechaInicioTorneo = config.FechaInicioTorneo,
            MaxPartidosPorDia = config.MaxPartidosPorDia,
            SeparacionEntreFechas = config.SeparacionEntreFechas
        };
    }
}