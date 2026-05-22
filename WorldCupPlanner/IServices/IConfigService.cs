using DTOs.Config;

namespace IServices;

public interface IConfigService
{
    TournamentConfigDTO GetConfig();
}