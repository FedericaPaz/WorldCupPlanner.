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
        {
            throw new ArgumentNullException(nameof(cruces));
        }

        _context.Cruces.AddRange(cruces);
        _context.SaveChanges();
    }

    public List<Cruce> GetAllCruces()
    {
        return _context.Cruces
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