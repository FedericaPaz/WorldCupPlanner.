using DTOs.Match;
using DTOs.Stadium;

namespace IServices.PageFacades;

public interface IMatchEditPageFacade
{
    MatchResponse GetMatchById(int id);
    List<StadiumRequest> GetAllStadiums();
    void UpdateMatchDate(int matchId, DateTime date);
    void UpdateMatchStadium(int matchId, string stadiumName);
    void SetMatchResult(int matchId, MatchResultRequest request);
    void SimulateMatch(int matchId, int seed, string userEmail);
    void RegisterLog(string userEmail, string category, string action, string detail);
}
