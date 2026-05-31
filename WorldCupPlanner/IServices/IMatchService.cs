using DTOs.Match;

namespace IServices;

public interface IMatchService
{
    MatchResponse CreateMatch(MatchRequest request);
    void SetMatchResult(int matchId, MatchResultRequest request);
    MatchResponse GetMatchById(int matchId);
    List<MatchResponse> GetAllMatches();
    void UpdateMatchDate(int matchId, DateTime date);
    void UpdateMatchStadium(int matchId, string stadiumName);
    void SimulateMatch(int matchId, int seed);
    void SimulateGroupStage();
}
