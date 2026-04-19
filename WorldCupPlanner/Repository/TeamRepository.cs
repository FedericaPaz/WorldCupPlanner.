using IRepository;
using Domain;

namespace Repository;

public class TeamRepository
{
    private readonly List<Team> _teams = new();

    public void Add(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));

        _teams.Add(team);
    }

    public Team? Get(string name)
    {
        return _teams.FirstOrDefault(t => t.Name == name);
    }

    public void Delete(Team team)
    {
        var existingTeam = _teams.FirstOrDefault(t => t.Name == team.Name);
        _teams.Remove(team);
    }

    public List<Team> GetAll()
    {
        return _teams;
    }
}