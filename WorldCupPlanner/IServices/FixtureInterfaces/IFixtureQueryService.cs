using DTOs.Fixture;
using Domain.Entities;

namespace IServices.FixtureInterfaces;

public interface IFixtureQueryService
{
    FixtureResponse GetFixture();
    bool HasFixture();
    List<StandingResponse> GetStandingsForGroup(char groupLetter);
    Fixture GetFixtureEntity();
}