using Domain;

namespace IRepository;

public interface IFixtureRepository
{
    void AddFixture(Fixture fixture);
    Fixture GetFixture();
    bool ContainsFixture(Fixture fixture);
    void RemoveFixture(Fixture fixture);
}