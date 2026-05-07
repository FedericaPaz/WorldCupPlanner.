using Domain;

namespace Repository;

public class LogRepository
{
    private readonly List<LogEntry> _logs = new();

    public void Add(LogEntry logEntry)
    {
        _logs.Add(logEntry);
    }

    public List<LogEntry> GetAll()
    {
        return _logs;
    }
}