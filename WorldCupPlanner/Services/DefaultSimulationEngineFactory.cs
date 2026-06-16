using Domain.Interfaces;
using IServices;

namespace Services;

public class DefaultSimulationEngineFactory : ISimulationEngineFactory
{
    private readonly IEnumerable<ISimulationEngine> _engines;

    public DefaultSimulationEngineFactory(IEnumerable<ISimulationEngine> engines)
    {
        _engines = engines ?? throw new ArgumentNullException(nameof(engines));
    }

    public ISimulationEngine Create(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var match = _engines.FirstOrDefault(e => e.Name == name);
            if (match is not null) return match;
        }

        return _engines.Last();
    }
}