using Domain.Interfaces;
using System.Collections.Generic;

namespace IServices
{
    public interface ISimulationEngineFactory
    {
        ISimulationEngine Create(string name);
    }
}