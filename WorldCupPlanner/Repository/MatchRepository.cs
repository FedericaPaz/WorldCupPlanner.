using Domain.Entities;
using IRepository;

namespace Repository;

public class MatchRepository : IMatchRepository
{
    private readonly WorldCupPlannerContext _context;

    public MatchRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddMatch(Match match)
    {
        EnsureNotNull(match);

        _context.Matches.Add(match);
        _context.SaveChanges();
    }

    public int GetNextId()
    {
        return _context.Matches.Any()
            ? _context.Matches.Max(match => match.IdMatch) + 1
            : 1;
    }

    public List<Match> GetAllMatches()
    {
        return _context.Matches.ToList();
    }

    public Match GetMatchById(int matchId)
    {
        EnsureValidId(matchId);

        var match = _context.Matches.FirstOrDefault(match => match.IdMatch == matchId);

        if (match == null)
            throw new KeyNotFoundException("No existe un partido con ese id.");

        return match;
    }

    public void ClearMatches()
    {
        _context.Matches.RemoveRange(_context.Matches);
        _context.SaveChanges();
    }

    private void EnsureNotNull(Match match)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));
    }

    private void EnsureValidId(int matchId)
    {
        if (matchId <= 0)
            throw new ArgumentException("El id debe ser mayor a 0.");
    }
}