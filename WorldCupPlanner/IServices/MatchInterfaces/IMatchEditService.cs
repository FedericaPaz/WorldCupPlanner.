using DTOs.Match;

namespace IServices.MatchInterfaces;

public interface IMatchEditService
{
    void UpdateMatchDate(int matchId, DateTime date);
    void UpdateMatchStadium(int matchId, string stadiumName);
    void SetMatchResult(int matchId, MatchResultRequest request);
}