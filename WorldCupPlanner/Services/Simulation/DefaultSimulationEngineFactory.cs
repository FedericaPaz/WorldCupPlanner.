using Domain.Interfaces;

namespace Services.Simulation;

public class DefaultSimulationEngineFactory : ISimulationEngineFactory
{
    public ISimulationEngine Create(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name) && name == new PureRandomSimulationEngine().Name)
            return new PureRandomSimulationEngine();

        return new ProbabilisticSimulationEngine();
    }
}