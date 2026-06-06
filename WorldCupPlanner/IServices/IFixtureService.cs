using DTOs.Fixture;
using Domain.Entities;

namespace IServices;

public interface IFixtureService
{
    FixtureResponse GetFixture();
    bool HasFixture();
    List<StandingResponse> GetStandingsForGroup(char groupLetter);
    void GenerateFixture();
    Fixture GetFixtureEntity();
}