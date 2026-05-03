using Domain;

namespace DomainTests.Stubs;

public class TeamStub : ITeam
{
    public string _teamName { get; }

    public TeamStub(string name)
    {
        _teamName = name;
    }
}