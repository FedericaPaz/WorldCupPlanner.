using Domain.Entities;
using Domain.Interfaces;

namespace IServices;

public interface ISimulationService
{
    (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed, ISimulationEngine engine);

    void SimulateGroupStage(List<Group> groups, int seed, ISimulationEngine engine);

    void SimulateCruces(List<Cruce> cruces, int seed, ISimulationEngine engine);
}