using Domain;
using IRepository;

namespace Repository;

public class MatchRepository : IMatchRepository
{
    private readonly List<Match> _matches = new();

    public void AddMatch(Match match)
    {
        _matches.Add(match);
    }

    public List<Match> GetAllMatches()
    {
        return _matches;
    }
    
    public Match GetMatchById(int matchId)
    {
        return _matches.First(m => m.IdMatch == matchId);
    }
    
}