using Domain.Interfaces;

namespace IServices;

public interface IMatchSimulationOrchestrator
{
    void SimulateMatch(int matchId, int seed);
    void SimulateMatch(int matchId, int seed, ISimulationEngine engine);
    void SimulateGroupStage();
    void SimulateGroupStage(ISimulationEngine engine);
}