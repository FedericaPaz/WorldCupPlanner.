using Domain;
using IRepository;

namespace Repository;

public class FixtureRepository : IFixtureRepository
{
    private Fixture? _fixture;
    
    public void AddFixture(Fixture fixture)
    {
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
        
        _fixture = fixture;
    }

    public Fixture GetFixture()
    {
        if (_fixture == null)
            throw new InvalidOperationException("No hay fixture generado");
        
        return _fixture;
    }

    public bool ContainsFixture()
    {
        return _fixture !=  null;
    }

    public void RemoveFixture(Fixture? fixture)
    {
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
        
        if (_fixture == fixture)
            _fixture = null;
    }
}