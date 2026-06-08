using Domain.Entities;
using Domain.Interfaces;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MatchRepository : IMatchRepository
{
    private readonly WorldCupPlannerContext _context;
    private readonly IDateTimeMatch _clock;

    public MatchRepository(WorldCupPlannerContext context, IDateTimeMatch clock)
    {
        _context = context;
        _clock = clock;
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
        var matches = _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Stadium)
            .AsNoTracking()
            .ToList();

        matches.ForEach(m => m.SetClock(_clock));
        return matches;
    }

    public Match GetMatchById(int matchId)
    {
        EnsureValidId(matchId);
        var match = _context.Matches
                        .Include(m => m.HomeTeam)
                        .Include(m => m.AwayTeam)
                        .Include(m => m.Stadium)
                        .Include(m => m.WinnerMatch)
                        .Include(m => m.LoserMatch)
                        .Include(m => m.Incidents)
                        .ThenInclude(i => i.Team)
                        .FirstOrDefault(m => m.IdMatch == matchId)
                    ?? throw new KeyNotFoundException("No existe un partido con ese id.");

        match.SetClock(_clock); 
        return match;
    }

    public void ClearMatches()
    {
        _context.Matches.RemoveRange(_context.Matches);
        _context.SaveChanges();
    }
    
    public void UpdateMatch(Match match)
    {
        Console.WriteLine("Entra en update match");
        if (match is null)
            throw new ArgumentNullException(nameof(match));

        var toDelete = _context.MatchIncidents
            .Where(i => i.MatchId == match.IdMatch)
            .ToList();
        _context.MatchIncidents.RemoveRange(toDelete);
        _context.SaveChanges();
        
        var existing = _context.Matches
                           .FirstOrDefault(m => m.IdMatch == match.IdMatch)
                       ?? throw new KeyNotFoundException($"No existe un partido con id {match.IdMatch}.");

        Console.WriteLine("existe");
        existing.SetClock(_clock);
        existing.SimulateResult(match.HomeGoals!.Value, match.AwayGoals!.Value);
        
        /*foreach (var incident in match.Incidents.ToList())
        {
            var trackedTeam = _context.Teams.Find(incident.Team.Name)
                              ?? throw new KeyNotFoundException($"No existe el equipo {incident.Team.Name}.");

            var newIncident = new MatchIncident(incident.Type, trackedTeam, incident.Quantity);
            existing.AddIncident(newIncident);
        }*/

        Console.WriteLine("antes de save changes");
        _context.SaveChanges();
        Console.WriteLine("despues de save changes");
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