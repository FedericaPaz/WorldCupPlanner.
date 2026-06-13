using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CrucesRepository : ICrucesRepository
{
    private readonly WorldCupPlannerContext _context;

    public CrucesRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddCruces(List<Cruce> cruces)
    {
        if (cruces is null)
            throw new ArgumentNullException(nameof(cruces));

        foreach (var cruce in cruces)
        {
            if (_context.Entry(cruce.Match).State == EntityState.Detached)
                _context.Attach(cruce.Match);

            if (_context.Entry(cruce.Match.HomeTeam).State == EntityState.Detached)
                _context.Attach(cruce.Match.HomeTeam);

            if (_context.Entry(cruce.Match.AwayTeam).State == EntityState.Detached)
                _context.Attach(cruce.Match.AwayTeam);

            if (_context.Entry(cruce.Match.Stadium).State == EntityState.Detached)
                _context.Attach(cruce.Match.Stadium);
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