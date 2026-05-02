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
        if (matchId <= 0)
            throw new ArgumentException("El id debe ser mayor a 0.");
        
        Match? match = _matches.FirstOrDefault(m => m.IdMatch == matchId);

        if (match == null)
            throw new KeyNotFoundException("No existe un partido con ese id.");

        return match;
    }

    
}