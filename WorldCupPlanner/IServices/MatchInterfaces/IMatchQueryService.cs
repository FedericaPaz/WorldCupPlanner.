using DTOs.Match;

namespace IServices.MatchInterfaces;

public interface IMatchQueryService
{
    MatchResponse GetMatchById(int matchId);
    List<MatchResponse> GetAllMatches();
    bool MatchesHaveBeenPlayed();
}