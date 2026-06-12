using Domain.Entities;

namespace Domain.Interfaces;

public interface ISimulationEngine
{
    string Name { get; }
    (int homeGoals, int awayGoals) SimulateMatch(int homeRanking, int awayRanking, int seed);
}