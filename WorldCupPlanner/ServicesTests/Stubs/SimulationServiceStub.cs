using Domain.Entities;
using Domain.Interfaces;
using IServices;

namespace ServicesTests.Stubs;

public class SimulationServiceStub : ISimulationService
{
    public string LastUsedEngineName { get; private set; } = string.Empty;

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed, ISimulationEngine engine)
    {
        LastUsedEngineName = engine.Name;
        return (2, 1);
    }

    public void SimulateGroupStage(List<Group> groups, int seed, ISimulationEngine engine)
    {
        LastUsedEngineName = engine.Name;
    }

    public void SimulateCruces(List<Cruce> cruces, int seed, ISimulationEngine engine)
    {
        LastUsedEngineName = engine.Name;
    }
}