using DTOs.Match;
using DTOs.Stadium;
using IServices;
using IServices.MatchInterfaces;
using IServices.PageFacades;

namespace Services.Facades;

public class MatchEditPageFacade : IMatchEditPageFacade
{
    private readonly IMatchQueryService _matchQuery;
    private readonly IMatchEditService _matchEdit;
    private readonly IStadiumService _stadiumService;
    private readonly ILogService _logService;
    private readonly ISimulationService _simulationService;

    public MatchEditPageFacade(
        IMatchQueryService matchQuery,
        IMatchEditService matchEdit,
        IStadiumService stadiumService,
        ILogService logService,
        ISimulationService simulationService)
    {
        _matchQuery = matchQuery;
        _matchEdit = matchEdit;
        _stadiumService = stadiumService;
        _logService = logService;
        _simulationService = simulationService;
    }

    public MatchResponse GetMatchById(int id) => _matchQuery.GetMatchById(id);

    public List<StadiumRequest> GetAllStadiums() => _stadiumService.GetAllStadiums();

    public void UpdateMatchDate(int matchId, DateTime date) =>
        _matchEdit.UpdateMatchDate(matchId, date);

    public void UpdateMatchStadium(int matchId, string stadiumName) =>
        _matchEdit.UpdateMatchStadium(matchId, stadiumName);

    public void SetMatchResult(int matchId, MatchResultRequest request) =>
        _matchEdit.SetMatchResult(matchId, request);

    public void SimulateMatch(int matchId, int seed, string userEmail)
    {
        var match = _matchQuery.GetMatchById(matchId);
        _simulationService.SimulateMatch(matchId, seed);
        _logService.Register(userEmail, "Partidos", "Modificación",
            $"Se simuló el resultado del partido {match.homeTeamName} vs {match.awayTeamName}");
    }

    public void RegisterLog(string userEmail, string category, string action, string detail) =>
        _logService.Register(userEmail, category, action, detail);
}
