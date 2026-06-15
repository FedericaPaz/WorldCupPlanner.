using Domain.Entities;

namespace IServices.FixtureInterfaces;

public interface IFixtureEditService
{
    void GenerateFixture(string simulationEngineName = "Probabilístico");
    void DeleteFixture();
}

