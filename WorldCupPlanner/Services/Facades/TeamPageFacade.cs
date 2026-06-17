using DTOs.Team;
using IServices;
using IServices.PageFacades;
using IServices.TeamInterfaces;

namespace Services.Facades;

public class TeamPageFacade : ITeamPageFacade
{
    private readonly ITeamQueryService _teamQuery;
    private readonly ITeamEditService _teamEdit;
    private readonly ILogService _logService;
    private readonly IConfigService _configService;

    public TeamPageFacade(
        ITeamQueryService teamQuery,
        ITeamEditService teamEdit,
        ILogService logService,
        IConfigService configService)
    {
        _teamQuery = teamQuery;
        _teamEdit = teamEdit;
        _logService = logService;
        _configService = configService;
    }

    public List<TeamDTO> GetAll() => _teamQuery.GetAll();

    public TeamDTO? Get(string name) => _teamQuery.Get(name);

    public List<ConfederationDTO> GetAllConfederations() => _teamQuery.GetAllConfederations();

    public void Add(TeamDTO dto, string userEmail)
    {
        _teamEdit.Add(dto);
        _logService.Register(userEmail, "Equipos", "Alta", $"Se creó el equipo {dto.Name}");
    }

    public void Update(string currentName, TeamDTO dto, string userEmail)
    {
        _teamEdit.Update(currentName, dto);
        _logService.Register(userEmail, "Equipos", "Edición", $"Se editó el equipo {currentName}");
    }

    public void Delete(string name, string userEmail)
    {
        _teamEdit.Delete(name);
        _logService.Register(userEmail, "Equipos", "Eliminación", $"Se eliminó el equipo {name}");
    }

    public void GenerateMissingTeams(string userEmail)
    {
        _teamEdit.GenerateMissingTeams();
        _logService.Register(userEmail, "Equipos", "Generación automática",
            $"Se generaron automáticamente los equipos faltantes con semilla {_configService.GetConfig().FillingSeed}");
    }
}
