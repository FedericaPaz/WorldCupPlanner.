using Domain.Entities;

namespace IServices;

public interface IRankingService
{
    void UpdateRankingsAfterMatch(Match match);
    void RevertRankingsAfterMatch(Match match);
}