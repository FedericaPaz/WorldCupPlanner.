using Domain;
using IRepository;

namespace Repository;

public class FixtureRepository : IFixtureRepository
{
    public void AddFixture(Fixture fixture)
    {
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
    }

    public Fixture GetFixture()
    {
        throw new NotImplementedException();
    }

    public bool ContainsFixture(Fixture fixture)
    {
        throw new NotImplementedException();
    }

    public void RemoveFixture(Fixture fixture)
    {
        throw new NotImplementedException();
    }
}