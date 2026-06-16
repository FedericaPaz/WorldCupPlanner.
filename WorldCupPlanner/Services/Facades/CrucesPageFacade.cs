using DTOs.Cruces;
using IServices;
using IServices.CrucesInterface;
using IServices.FixtureInterfaces;
using IServices.MatchInterfaces;
using IServices.PageFacades;

namespace Services.Facades;

public class CrucesPageFacade : ICrucesPageFacade
{
    private readonly ICruceQueryService _cruceQuery;
    private readonly ICruceEditService _cruceEdit;
    private readonly IFixtureQueryService _fixtureQuery;
    private readonly IMatchQueryService _matchQuery;
    private readonly ILogService _logService;
    private readonly IConfigService _configService;

    public CrucesPageFacade(
        ICruceQueryService cruceQuery,
        ICruceEditService cruceEdit,
        IFixtureQueryService fixtureQuery,
        IMatchQueryService matchQuery,
        ILogService logService,
        IConfigService configService)
    {
        _cruceQuery = cruceQuery;
        _cruceEdit = cruceEdit;
        _fixtureQuery = fixtureQuery;
        _matchQuery = matchQuery;
        _logService = logService;
        _configService = configService;
    }

    public bool HasFixture() => _fixtureQuery.HasFixture();

    public List<CruceResponse> GetAllCruces() => _cruceQuery.GetAllCruces();

    public int GetPendingMatchCount()
    {
        var partidos = _matchQuery.GetAllMatches();
        return partidos.Count(m => !m.homeGoals.HasValue);
    }

    public bool AllGroupMatchesPlayed()
    {
        var partidos = _matchQuery.GetAllMatches();
        return partidos.Any() && partidos.All(m => m.homeGoals.HasValue);
    }

    public void GenerateRoundOf32(string userEmail)
    {
        _cruceEdit.GenerateRoundOf32();
        _logService.Register(userEmail, "Cruces", "Sorteo",
            $"Se realizó el sorteo de cruces de 32avos de final con semilla {_configService.GetConfig().SemillaCrucesFase}");
    }

    public void GenerateNextRound(string userEmail)
    {
        _cruceEdit.GenerateNextRound();
        _logService.Register(userEmail, "Cruces", "Sorteo",
            $"Se realizó el sorteo de la siguiente ronda de cruces con semilla {_configService.GetConfig().SemillaCrucesFase}");
    }
}
