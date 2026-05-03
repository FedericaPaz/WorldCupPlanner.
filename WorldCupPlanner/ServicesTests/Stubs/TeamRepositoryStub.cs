using Domain;
using DomainTests.Stubs;
using IRepository;

namespace ServicesTests.Stubs;

public class TeamRepositoryStub : ITeamRepository
{
    private readonly List<ITeam> _teams = new();

    public void Add(ITeam team)
    {
        _teams.Add(team);
    }

    public ITeam GetByName(string name)
    {
        var team = _teams.FirstOrDefault(t => t.Name == name);

        if (team == null)
            throw new KeyNotFoundException("Team not found");

        return team;
    }
}