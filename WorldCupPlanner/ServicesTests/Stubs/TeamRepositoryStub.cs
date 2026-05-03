using Domain;
using IRepository;

namespace ServicesTests.Stubs;

public class TeamRepositoryStub : ITeamRepository
{
    private readonly List<Team> _teams = new();

    public void Add(Team team)
    {
        _teams.Add(team);
    }

    public Team? Get(string name)
    {
        return _teams.FirstOrDefault(t => t.Name == name);
    }

    public void Delete(Team team)
    {
        _teams.Remove(team);
    }

    public List<Team> GetAll()
    {
        return new List<Team>(_teams);
    }
}