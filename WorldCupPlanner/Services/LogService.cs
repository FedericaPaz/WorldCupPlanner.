using Domain;
using IRepository;

namespace Services;

public class LogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public void Register(string userEmail, string category, string action, string detail)
    {
        var logEntry = new LogEntry(userEmail, category, action, detail);
        _logRepository.Add(logEntry);
    }

    public List<LogEntry> GetAll()
    {
        return _logRepository.GetAll();
    }
}