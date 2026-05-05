using Domain;

namespace DomainTests.Stubs;

public class TeamStub : ITeam
{
    public string Name { get; }

    public TeamStub(string name)
    {
        Name = name;
    }
}