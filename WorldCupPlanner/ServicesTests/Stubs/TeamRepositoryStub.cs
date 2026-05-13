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

    public void Update(Team team)
    {
        var existingTeam = _teams.FirstOrDefault(t => t.Name == team.Name);

        if (existingTeam is null)
            throw new InvalidOperationException($"Team {team.Name} does not exist.");

        _teams.Remove(existingTeam);
        _teams.Add(team);
    }

    public void Delete(Team team)
    {
        _teams.Remove(team);
    }

    public List<Team> GetAll()
    {
        return new List<Team>(_teams);
    }

    public void Clear()
    {
        _teams.Clear();
    }
}