using Domain.Entities;
using DTOs.Config;

namespace Services.Mappers;

public static class ConfigMapper
{
    public static TournamentConfigDTO ToDTO(TournamentConfig config)
    {
        return new TournamentConfigDTO
        {
            FixtureSeed = config.FixtureSeed,
            FillingSeed = config.FillingSeed,
            PhaseDrawSeed = config.PhaseDrawSeed,
            SimulationSeed = config.SimulationSeed,
            TournamentStartDate = config.TournamentStartDate,
            MaxMatchesPerDay = config.MaxMatchesPerDay,
            DaysBetweenMatchdays = config.DaysBetweenMatchdays
        };
    }
}