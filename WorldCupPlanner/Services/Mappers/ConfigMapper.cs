using Domain.Entities;
using DTOs.Config;

namespace Services.Mappers;

public static class ConfigMapper
{
    public static TournamentConfigDTO ToDTO(TournamentConfig config)
    {
        return new TournamentConfigDTO
        {
            SemillaFixture = config.FixtureSeed,
            SemillaCompletar = config.FillingSeed,
            SemillaCrucesFase = config.PhaseDrawSeed,
            SemillaSimulation = config.SimulationSeed,
            FechaInicioTorneo = config.TournamentStartDate,
            MaxPartidosPorDia = config.MaxMatchesPerDay,
            SeparacionEntreFechas = config.DaysBetweenMatchdays
        };
    }
}