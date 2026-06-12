using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EfCrucesRepository : ICrucesRepository
{
    private readonly WorldCupPlannerContext _context;

    public EfCrucesRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddCruces(List<Cruce> cruces)
    {
        if (cruces is null)
            throw new ArgumentNullException(nameof(cruces));

        foreach (var entry in _context.ChangeTracker.Entries().ToList())
            entry.State = EntityState.Detached;

        foreach (var cruce in cruces)
        {
            if (_context.Entry(cruce.Match).State == EntityState.Detached)
                _context.Entry(cruce.Match).State = EntityState.Unchanged;

            if (_context.Entry(cruce.Match.HomeTeam).State == EntityState.Detached)
                _context.Entry(cruce.Match.HomeTeam).State = EntityState.Unchanged;

            if (_context.Entry(cruce.Match.AwayTeam).State == EntityState.Detached)
                _context.Entry(cruce.Match.AwayTeam).State = EntityState.Unchanged;

            if (_context.Entry(cruce.Match.Stadium).State == EntityState.Detached)
                _context.Entry(cruce.Match.Stadium).State = EntityState.Unchanged;
        }

        _context.Cruces.AddRange(cruces);
        _context.SaveChanges();
    }

    public List<Cruce> GetAllCruces()
    {
        return _context.Cruces
            .Include(c => c.Match)
            .ThenInclude(m => m.HomeTeam)
            .Include(c => c.Match)
            .ThenInclude(m => m.AwayTeam)
            .Include(c => c.Match)
            .ThenInclude(m => m.Stadium)
            .Include(c => c.Match)
            .ThenInclude(m => m.WinnerMatch)
            .Include(c => c.Match)
            .ThenInclude(m => m.LoserMatch)
            .AsNoTracking()
            .ToList();
    }

    public bool HasCruces()
    {
        return _context.Cruces.Any();
    }

    public void RemoveCruces()
    {
        _context.Cruces.RemoveRange(_context.Cruces);
        _context.SaveChanges();
    }
}