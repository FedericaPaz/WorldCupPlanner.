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

    public List<Team> GetAll()
    {
        return _teams;
    }
}