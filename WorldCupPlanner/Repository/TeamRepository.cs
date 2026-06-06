using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class TeamRepository : ITeamRepository
{
    private readonly WorldCupPlannerContext _context;

    public TeamRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void Add(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        _context.Teams.Add(team);
        _context.SaveChanges();
    }

    public Team? Get(string name)
    {
        return _context.Teams
            .AsNoTracking()
            .FirstOrDefault(t => t.Name == name);
    }

    public void Update(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        var existing = _context.Teams.FirstOrDefault(t => t.Name == team.Name)
                       ?? throw new InvalidOperationException($"Team {team.Name} does not exist.");

        existing.UpdateFifaRanking(team.FifaRanking);
        existing.UpdateCurrentRanking(team.CurrentRanking);
        existing.UpdateConfederation(team.Confederation);

        _context.SaveChanges();
    }

    public void Delete(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        var existing = _context.Teams.FirstOrDefault(t => t.Name == team.Name)
                       ?? throw new InvalidOperationException($"Team {team.Name} does not exist.");

        _context.Teams.Remove(existing);
        _context.SaveChanges();
    }

    public List<Team> GetAll()
    {
        return _context.Teams
            .AsNoTracking()
            .ToList();
    }

    public void Clear()
    {
        _context.Teams.RemoveRange(_context.Teams);
        _context.SaveChanges();
    }
}