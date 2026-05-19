using Domain.Interfaces;

namespace ServicesTests.Stubs;

public class ClockStub : IDateTimeMatch
{
    private readonly DateTime _now;
    public ClockStub(DateTime now)
    {
        _now = now;
    }
    public DateTime Now => _now;
}