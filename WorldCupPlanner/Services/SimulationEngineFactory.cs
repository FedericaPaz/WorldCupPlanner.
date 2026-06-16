using Domain.Interfaces;
using IServices;

namespace Services;

public class SimulationEngineFactory : ISimulationEngineFactory
{
    private readonly Dictionary<string, ISimulationEngine> _engines;

    public SimulationEngineFactory(IEnumerable<ISimulationEngine> engines)
    {
        if (engines == null) throw new ArgumentNullException(nameof(engines));
        _engines = engines.ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);
    }

    public ISimulationEngine Create(string name)
    {
        if (_engines.TryGetValue(name, out var engine)) return engine;
        throw new InvalidOperationException($"No hay motor registrado con el nombre '{name}'.");
    }

    public IEnumerable<string> ListEngines() => _engines.Keys;
}
