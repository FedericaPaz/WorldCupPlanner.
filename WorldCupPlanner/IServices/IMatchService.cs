using DTOs;

namespace IServices;

public interface IMatchService
{
    MatchResponse CreateMatch(MatchRequest request);
    void SetMatchResult(int matchId, int homeGoals, int awayGoals);
}
