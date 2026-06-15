using DTOs.Match;

namespace IServices.MatchInterfaces;

public interface IMatchCreateService
{
    MatchResponse CreateMatch(MatchRequest request);
}