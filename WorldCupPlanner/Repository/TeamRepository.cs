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

    public List<Team> GetAll()
    {
        return _teams;
    }
}