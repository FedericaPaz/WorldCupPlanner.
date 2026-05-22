using Domain.Entities;

namespace IRepository;

public interface IFixtureRepository
{
    void AddFixture(Fixture fixture);
    Fixture GetFixture();
    bool ContainsFixture();
    void RemoveFixture(Fixture? fixture);
}