using Domain;

namespace DomainTests.Stubs;

public class StadiumStub : IStadium
{
    public string _stadiumName { get; }

    public StadiumStub(string name)
    {
        _stadiumName = name;
    }
}