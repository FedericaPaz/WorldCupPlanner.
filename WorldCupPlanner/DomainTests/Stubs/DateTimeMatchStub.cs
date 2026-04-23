namespace DomainTests.Stubs;

public class DateTimeMatchStub
{
    private readonly DateTime _dateTime;
    public  DateTimeMatchStub(DateTime dateTime)
    {
        _dateTime = dateTime;
    }
    
    public DateTime Now => _dateTime;
}