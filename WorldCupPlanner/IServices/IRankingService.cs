using Domain.Entities;

namespace IServices;

public interface IRankingService
{
    void UpdateRankingsAfterMatch(Match match, int  homeGoals, int awayGoals);
    void RevertRankingsAfterMatch(Match match);
}