using Domain;
using DTOs;
using IServices;
using Services.Mappers;

namespace Services;

public class ConfigService : IConfigService
{
    private readonly TournamentConfig _config;

    public ConfigService(TournamentConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _config = config;
    }

    public TournamentConfigDTO GetConfig()
    {
        return ConfigMapper.ToDTO(_config);
    }
}