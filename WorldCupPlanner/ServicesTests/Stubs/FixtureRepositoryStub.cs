using Domain.Entities;
using IRepository;

namespace ServicesTests.Stubs;

public class FixtureRepositoryStub : IFixtureRepository
{
    private Fixture _fixture;

    public void AddFixture(Fixture fixture)
    {
        _fixture = fixture;
    }

    public Fixture GetFixture()
    {
        return _fixture;
    }

    public bool ContainsFixture()
    {
        return _fixture != null;
    }

    public void RemoveFixture(Fixture? fixture)
    {
        _fixture = null;
    }
}