using Domain;
using DomainTests.Stubs;
using IRepository;

namespace ServicesTests.Stubs;

public class TeamRepositoryStub : ITeamRepository
{
    public ITeam GetByName(string name)
    {
        return new TeamStub(name);
    }
}