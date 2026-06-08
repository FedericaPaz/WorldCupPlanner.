using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class FixtureRepository : IFixtureRepository
{
    private readonly WorldCupPlannerContext _context;
    
    public FixtureRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }
    
    public void AddFixture(Fixture fixture)
    {
        if (fixture == null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        var existing = _context.Fixtures.ToList();
        if (existing.Any())
        {
            foreach (var f in existing)
            {
                foreach (var g in f.Groups)
                {
                    var matches = _context.Matches
                        .Where(m => m.Group == g.GroupLetter)
                        .ToList();
                    _context.Matches.RemoveRange(matches);
                }
                _context.Groups.RemoveRange(f.Groups);
            }
            _context.Fixtures.RemoveRange(existing);
            _context.SaveChanges();
        }
        
        _context.Fixtures.Add(fixture);
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Inner: {ex.InnerException?.Message}");
            throw;
        }
    }

    public Fixture GetFixture()
    {
        return _context.Fixtures
                   .Include(f => f.Groups)
                   .FirstOrDefault()
               ?? throw new InvalidOperationException("No hay fixture generado");
    }

    public bool ContainsFixture()
    {
        return _context.Fixtures.Any();
    }

    public void RemoveFixture(Fixture? fixture)
    {
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
        
        var existingFixture = _context.Fixtures.FirstOrDefault(f => f.Id == fixture.Id);

        if (existingFixture != null)
        {
            _context.Fixtures.Remove(existingFixture);
            _context.SaveChanges();
        }
    }
}