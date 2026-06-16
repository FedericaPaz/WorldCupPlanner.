using Domain.Interfaces;
using IServices;

namespace ServicesTests.Stubs;

public class TestEngineFactory : ISimulationEngineFactory
{
    private readonly ISimulationEngine _engine;

    public TestEngineFactory(ISimulationEngine engine)
    {
        _engine = engine;
    }

    public ISimulationEngine Create(string? name)
    {
        return _engine;
    }
}
