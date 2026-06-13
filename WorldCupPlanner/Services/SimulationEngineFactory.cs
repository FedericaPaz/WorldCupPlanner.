using System;
using System.Collections.Generic;
using Domain.Interfaces;
using IServices;

namespace Services
{
    public class SimulationEngineFactory : ISimulationEngineFactory
    {
        private readonly Dictionary<string, Func<ISimulationEngine>> _factories =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly object _lock = new();

        public void Register(string name, Func<ISimulationEngine> factory)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre del motor no puede ser vacío.", nameof(name));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            lock (_lock)
            {
                _factories[name] = factory;
            }
        }

        public IEnumerable<string> ListEngines() => _factories.Keys;

        public bool TryCreate(string name, out ISimulationEngine engine)
        {
            engine = null;
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (_factories.TryGetValue(name, out var f))
            {
                engine = f();
                return true;
            }
            return false;
        }

        public ISimulationEngine Create(string name)
        {
            if (TryCreate(name, out var engine)) return engine;
            throw new InvalidOperationException($"No hay motor registrado con el nombre '{name}'.");
        }
    }
}
