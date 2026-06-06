using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

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
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        foreach (var entry in _context.ChangeTracker.Entries().ToList())
            entry.State = EntityState.Detached;

        if (_context.Entry(match.HomeTeam).State == EntityState.Detached)
            _context.Entry(match.HomeTeam).State = EntityState.Unchanged;

        if (_context.Entry(match.AwayTeam).State == EntityState.Detached)
            _context.Entry(match.AwayTeam).State = EntityState.Unchanged;

        if (_context.Entry(match.Stadium).State == EntityState.Detached)
            _context.Entry(match.Stadium).State = EntityState.Unchanged;

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
        return _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Stadium)
            .AsNoTracking()
            .ToList();
    }

    public Match GetMatchById(int matchId)
    {
        EnsureValidId(matchId);

        return _context.Matches
                   .Include(m => m.HomeTeam)
                   .Include(m => m.AwayTeam)
                   .Include(m => m.Stadium)
                   .Include(m => m.WinnerMatch)
                   .Include(m => m.LoserMatch)
                   .Include(m => m.Incidents)
                   .ThenInclude(i => i.Team)
                   .FirstOrDefault(m => m.IdMatch == matchId)
               ?? throw new KeyNotFoundException("No existe un partido con ese id.");
    }

    public void ClearMatches()
    {
        _context.Matches.RemoveRange(_context.Matches);
        _context.SaveChanges();
    }
    
    public void UpdateMatch(Match match)
    {
        if (match is null)
            throw new ArgumentNullException(nameof(match));

        foreach (var entry in _context.ChangeTracker.Entries().ToList())
            entry.State = EntityState.Detached;

        var existing = _context.Matches
                           .Include(m => m.Incidents)
                           .FirstOrDefault(m => m.IdMatch == match.IdMatch)
                       ?? throw new KeyNotFoundException($"No existe un partido con id {match.IdMatch}.");

        if (_context.Entry(match.HomeTeam).State == EntityState.Detached)
            _context.Entry(match.HomeTeam).State = EntityState.Unchanged;

        if (_context.Entry(match.AwayTeam).State == EntityState.Detached)
            _context.Entry(match.AwayTeam).State = EntityState.Unchanged;

        existing.SimulateResult(match.HomeGoals!.Value, match.AwayGoals!.Value);

        foreach (var incident in match.Incidents)
        {
            if (_context.Entry(incident.Team).State == EntityState.Detached)
                _context.Entry(incident.Team).State = EntityState.Unchanged;

            existing.AddIncident(incident);
        }

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