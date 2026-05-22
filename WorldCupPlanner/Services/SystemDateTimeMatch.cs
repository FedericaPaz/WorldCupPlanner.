using Domain.Interfaces;

namespace Services;

public class SystemDateTimeMatch : IDateTimeMatch
{ 
    public DateTime Now => DateTime.Now;
     
}