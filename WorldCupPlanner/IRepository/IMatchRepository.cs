using Domain.Entities;

namespace IRepository;

public interface IMatchRepository
{
    void AddMatch(Match match);
    int GetNextId();
    List<Match> GetAllMatches();
    Match GetMatchById(int matchId);
    void ClearMatches();
}