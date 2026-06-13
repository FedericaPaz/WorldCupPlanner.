using Domain.Interfaces;

namespace Services.Simulation;

public interface ISimulationEngineFactory
{
    ISimulationEngine Create(string? name);
}