using Domain.Entities;
using IRepository;

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
        
        var existing = _context.Fixtures.ToList();
        if (existing.Any())
            _context.Fixtures.RemoveRange(existing);
        
        _context.Fixtures.Add(fixture);
        _context.SaveChanges();
    }

    public Fixture GetFixture()
    {
        var fixture = _context.Fixtures.FirstOrDefault();
        if (fixture == null)
            throw new InvalidOperationException("No hay fixture generado");

        return fixture;
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