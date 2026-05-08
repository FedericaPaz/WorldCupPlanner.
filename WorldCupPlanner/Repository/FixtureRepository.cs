using Domain;
using IRepository;

namespace Repository;

public class FixtureRepository : IFixtureRepository
{
    private readonly List<Fixture> _fixture = new();
    public void AddFixture(Fixture fixture)
    {
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
        _fixture.Add(fixture);
    }

    public Fixture GetFixture()
    {
        return _fixture[0];
    }

    public bool ContainsFixture(Fixture fixture)
    {
        return _fixture.Contains(fixture);
    }

    public void RemoveFixture(Fixture fixture)
    {
        throw new NotImplementedException();
    }
}