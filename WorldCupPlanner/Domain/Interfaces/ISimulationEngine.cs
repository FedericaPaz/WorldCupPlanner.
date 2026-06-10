namespace Domain.Interfaces;

public interface ISimulationEngine
{
    string Name { get; }
    (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed);
}