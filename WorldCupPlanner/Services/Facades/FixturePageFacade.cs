using DTOs.Fixture;
using IServices;
using IServices.FixtureInterfaces;
using IServices.MatchInterfaces;
using IServices.PageFacades;
using IServices.TeamInterfaces;

namespace Services.Facades;

public class FixturePageFacade : IFixturePageFacade
{
    private readonly IFixtureQueryService _fixtureQuery;
    private readonly IFixtureEditService _fixtureEdit;
    private readonly ITeamQueryService _teamQuery;
    private readonly IStadiumService _stadiumService;
    private readonly ILogService _logService;
    private readonly IConfigService _configService;
    private readonly IMatchQueryService _matchQuery;

    public FixturePageFacade(
        IFixtureQueryService fixtureQuery,
        IFixtureEditService fixtureEdit,
        ITeamQueryService teamQuery,
        IStadiumService stadiumService,
        ILogService logService,
        IConfigService configService,
        IMatchQueryService matchQuery)
    {
        _fixtureQuery = fixtureQuery;
        _fixtureEdit = fixtureEdit;
        _teamQuery = teamQuery;
        _stadiumService = stadiumService;
        _logService = logService;
        _configService = configService;
        _matchQuery = matchQuery;
    }

    public bool HasFixture() => _fixtureQuery.HasFixture();
    public FixtureResponse GetFixture() => _fixtureQuery.GetFixture();
    public int GetTeamCount() => _teamQuery.GetAll().Count;
    public int GetStadiumCount() => _stadiumService.GetAllStadiums().Count;
    public bool MatchesHaveBeenPlayed() => _matchQuery.MatchesHaveBeenPlayed();

    public void GenerateFixture(string engineName, string userEmail)
    {
        _fixtureEdit.GenerateFixture(engineName);
        _logService.Register(userEmail, "Fixture", "Generación",
            $"Se generó el fixture del torneo con semilla {_configService.GetConfig().SemillaFixture}");
    }

    public void RegenerateFixture(string engineName, string userEmail)
    {
        _fixtureEdit.GenerateFixture(engineName);
        _logService.Register(userEmail, "Fixture", "Generación",
            $"Se regeneró el fixture del torneo con semilla {_configService.GetConfig().SemillaFixture}");
    }

    public void DeleteFixture(string userEmail)
    {
        _fixtureEdit.DeleteFixture();
        _logService.Register(userEmail, "Fixture", "Generación",
            $"Se elimino el fixture generado con:  {_configService.GetConfig().SemillaFixture}");
    }
}
