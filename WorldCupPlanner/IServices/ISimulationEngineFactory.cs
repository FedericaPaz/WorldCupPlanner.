using Domain.Interfaces;

namespace IServices
{
    public interface ISimulationEngineFactory
    {
        ISimulationEngine Create(string name);
    }
}