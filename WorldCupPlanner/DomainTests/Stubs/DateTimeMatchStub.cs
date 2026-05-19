using Domain.Interfaces;

namespace DomainTests.Stubs;

public class DateTimeMatchStub : IDateTimeMatch
{
    public DateTime Now { get; set; }
    public DateTimeMatchStub(DateTime now)
    {
        Now = now;
    }
}