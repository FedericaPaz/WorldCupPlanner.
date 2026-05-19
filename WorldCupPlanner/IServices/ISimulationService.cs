using Domain.Entities;

namespace IServices;

public interface ISimulationService
{
    (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed);
    void SimulateGroupStage(List<Group> groups, int seed);
    void SimulateCruces(List<Cruce> cruces, int seed);
}