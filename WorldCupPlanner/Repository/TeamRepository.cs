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
        
        existing.UpdateFlag(team.FlagBase64); 

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

    public void Rename(string oldName, Team newTeam)
    {
        if (string.IsNullOrWhiteSpace(oldName))
            throw new ArgumentException("oldName cannot be empty", nameof(oldName));
        if (newTeam is null)
            throw new ArgumentNullException(nameof(newTeam));

        var provider = _context.Database.ProviderName ?? string.Empty;
        var useTransaction = !provider.Contains("InMemory", StringComparison.OrdinalIgnoreCase);

        if (useTransaction)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                PerformRename(oldName, newTeam);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        else
        {
            // In-memory provider: transactions are ignored, perform operations directly
            PerformRename(oldName, newTeam);
        }
    }

    private void PerformRename(string oldName, Team newTeam)
    {
        var existing = _context.Teams.FirstOrDefault(t => t.Name == oldName)
                       ?? throw new InvalidOperationException($"Team {oldName} does not exist.");

        // Add the new team entity
        _context.Teams.Add(newTeam);
        _context.SaveChanges();

        // Update Matches that reference the old team to reference the new team using FK properties
        var matches = _context.Matches
        .Include(m => m.HomeTeam)
        .Include(m => m.AwayTeam)
        .Include(m => m.WinnerMatch)
        .Include(m => m.LoserMatch)
        .Where(m => (m.HomeTeam != null && m.HomeTeam.Name == oldName)
                    || (m.AwayTeam != null && m.AwayTeam.Name == oldName)
                    || (m.WinnerMatch != null && m.WinnerMatch.Name == oldName)
                    || (m.LoserMatch != null && m.LoserMatch.Name == oldName))
        .ToList();

        foreach (var match in matches)
        {
            if (match.HomeTeam != null && match.HomeTeam.Name == oldName)
                _context.Entry(match).Property("HomeTeamName").CurrentValue = newTeam.Name;
            if (match.AwayTeam != null && match.AwayTeam.Name == oldName)
                _context.Entry(match).Property("AwayTeamName").CurrentValue = newTeam.Name;
            if (match.WinnerMatch != null && match.WinnerMatch.Name == oldName)
                _context.Entry(match).Property("WinnerMatchName").CurrentValue = newTeam.Name;
            if (match.LoserMatch != null && match.LoserMatch.Name == oldName)
                _context.Entry(match).Property("LoserMatchName").CurrentValue = newTeam.Name;
        }

        // Update MatchIncidents referencing the old team
        var incidents = _context.MatchIncidents
            .Where(i => i.TeamName == oldName)
            .ToList();

        foreach (var incident in incidents)
        {
            _context.Entry(incident).Property("TeamName").CurrentValue = newTeam.Name;
        }

        _context.SaveChanges();

        // Remove old team
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