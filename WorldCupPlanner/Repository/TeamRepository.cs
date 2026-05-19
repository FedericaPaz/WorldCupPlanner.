using IRepository;
using Domain.Entities;

namespace Repository;

public class TeamRepository :  ITeamRepository
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
        if (team is null)
            throw new ArgumentNullException(nameof(team));
        
        var existingTeam = _teams.FirstOrDefault(t => t.Name == team.Name);
        
        if (existingTeam is null)
            throw new InvalidOperationException($"Team {team.Name} does not exist.");
            
        _teams.Remove(existingTeam);
    }

    public List<Team> GetAll()
    {
        return new List<Team>(_teams);
    }
    
    public void Update(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        var existingTeam = _teams.FirstOrDefault(t => t.Name == team.Name);

        if (existingTeam is null)
            throw new InvalidOperationException($"Team {team.Name} does not exist.");

        _teams.Remove(existingTeam);
        _teams.Add(team);
    }

    public void Clear()
    {
        _teams.Clear();
    }
}