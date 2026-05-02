using Domain;

namespace IRepository;

public interface IMatchRepository
{
    void AddMatch(Match match);
    List<Match> GetAllMatches();
}