namespace Domain.Interfaces;

public interface ISimulationEngine
{
    string Name { get; }
    (int homeGoals, int awayGoals) Simulate(int homeRanking, int awayRanking, int seed);
}