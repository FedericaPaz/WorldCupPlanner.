using Domain.Entities;
using IRepository;

namespace Repository;

public class LogRepository : ILogRepository
{
    private readonly WorldCupPlannerContext _context;
    
    public LogRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }
    
    private readonly List<LogEntry> _logs = new();

    public void Add(LogEntry logEntry)
    {
        if (logEntry is null)
            throw new ArgumentNullException(nameof(logEntry));

        _context.LogEntries.Add(logEntry);
        _context.SaveChanges();
    }
    
    public List<LogEntry> GetAll()
    {
        return _context.LogEntries
            .OrderByDescending(log => log.Timestamp)
            .ToList();
    }
}