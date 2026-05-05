using Domain;

namespace DomainTests.Stubs;

public class StadiumStub : IStadium
{
    public string StadiumName { get; }

    public StadiumStub(string name)
    {
        StadiumName = name;
    }
}