using Domain.Entities;

namespace IRepository;

public interface IMatchRepository
{
    void AddMatch(Match match);
    List<Match> GetAllMatches();
    Match GetMatchById(int matchId);
    void ClearMatches();
}