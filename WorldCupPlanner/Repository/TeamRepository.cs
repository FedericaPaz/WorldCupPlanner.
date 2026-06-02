using IRepository;
using Domain.Entities;

namespace Repository;

public class TeamRepository :  ITeamRepository
{
    private readonly WorldCupPlannerContext _context;
    
    public TeamRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void Add(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));

        _context.Add(team);
        _context.SaveChanges();
    }

    public Team? Get(string name)
    {
        return _context.Teams.FirstOrDefault(t => t.Name == name);
    }
    
    public void Delete(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));
        
        var existingTeam = _context.Teams.FirstOrDefault(t => t.Name == team.Name);
        
        if (existingTeam is null)
            throw new InvalidOperationException($"Team {team.Name} does not exist.");
            
        _context.Teams.Remove(existingTeam);
        _context.SaveChanges();
    }

    public List<Team> GetAll()
    {
        return _context.Teams.ToList();
    }
    
    public void Update(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        var existingTeam = _context.Teams.FirstOrDefault(t => t.Name == team.Name);

        if (existingTeam is null)
            throw new InvalidOperationException($"Team {team.Name} does not exist.");

        _context.Remove(existingTeam);
        _context.Add(team);
        _context.SaveChanges();
    }

    public void Clear()
    {
        _context.Teams.RemoveRange(_context.Teams);
        _context.SaveChanges();
    }
}