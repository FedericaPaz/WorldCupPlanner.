using Domain.Interfaces;
using System.Collections.Generic;

namespace IServices
{
    public interface ISimulationEngineFactory
    {
        IEnumerable<string> ListEngines();
        bool TryCreate(string name, out ISimulationEngine engine);
        ISimulationEngine Create(string name); // lanza InvalidOperationException con mensaje en español si no existe
    }
}