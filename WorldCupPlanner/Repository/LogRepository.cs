using Domain;

namespace Repository;

public class LogRepository
{
    private readonly List<LogEntry> _logs = new();

    public void Add(LogEntry logEntry)
    {
        if (logEntry is null)
        {
            throw new ArgumentNullException(nameof(logEntry));
        }

        _logs.Add(logEntry);
    }

    public List<LogEntry> GetAll()
    {
        return _logs
            .OrderByDescending(log => log.Timestamp)
            .ToList();
    }
    
    
}