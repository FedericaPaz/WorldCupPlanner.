using DTOs.Fixture;
using Domain.Entities;

namespace IServices;

public interface IFixtureService
{
    FixtureResponse GetFixture();
    bool HasFixture();
    List<StandingResponse> GetStandingsForGroup(char groupLetter);
    void GenerateFixture(string simulationEngineName = "Probabilístico");
    Fixture GetFixtureEntity();
    void DeleteFixture(Fixture fixture);
}