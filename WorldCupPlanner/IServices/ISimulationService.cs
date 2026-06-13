using Domain.Entities;
using Domain.Interfaces;

namespace IServices;

public interface ISimulationService
{
    void SimulateMatch(int matchId, int seed);
    void SimulateMatch(int matchId, int seed, ISimulationEngine engine);
    void SimulateGroupStage();
    void SimulateGroupStage(ISimulationEngine engine);

    (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed, ISimulationEngine engine);
    void SimulateCruces(List<Cruce> cruces, int seed, ISimulationEngine engine);
}