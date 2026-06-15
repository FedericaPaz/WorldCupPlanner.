
namespace IServices;

public interface ISimulationService
{
    void SimulateMatch(int matchId, int seed);
    void SimulateGroupStage();
}