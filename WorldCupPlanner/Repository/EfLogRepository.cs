using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EfLogRepository : ILogRepository
{
    private readonly WorldCupPlannerContext _context;

    public EfLogRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void Add(LogEntry logEntry)
    {
        _context.LogEntries.Add(logEntry);
        _context.SaveChanges();
    }

    public List<LogEntry> GetAll()
    {
        return _context.LogEntries
            .AsNoTracking()
            .ToList();
    }
}