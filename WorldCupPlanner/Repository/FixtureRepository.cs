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
            throw new ArgumentNullException(nameof(fixture));
        
        var existingFixtures = _context.Fixtures.ToList(); 

        if (existingFixtures.Any())
        {
            _context.Fixtures.RemoveRange(existingFixtures);
            _context.SaveChanges();
        }
    
        _context.Fixtures.Add(fixture);
    
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("An error occurred while saving the fixture to the database.", ex);
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