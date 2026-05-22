using Domain.Entities;
using IRepository;

namespace Repository;

public class MatchRepository : IMatchRepository
{
    private readonly List<Match> _matches = new();

    public void AddMatch(Match match)
    {
        EnsureNotNull(match);
        _matches.Add(match);
    }

    private void EnsureNotNull(Match match)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));
    }

    public List<Match> GetAllMatches()
    {
        return _matches.ToList();
    }

    public Match GetMatchById(int matchId)
    {
        EnsureValidId(matchId);

        Match? match = _matches.FirstOrDefault(m => m.IdMatch == matchId);

        if (match == null)
            throw new KeyNotFoundException("No existe un partido con ese id.");

        return match;
    }

    public void ClearMatches()
    {
        _matches.Clear();
    }

    private void EnsureValidId(int matchId)
    {
        if (matchId <= 0)
            throw new ArgumentException("El id debe ser mayor a 0.");
    }
}