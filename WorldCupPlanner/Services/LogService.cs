using Domain;
using IRepository;
using IServices;

namespace Services;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        if (logRepository is null)
        {
            throw new ArgumentNullException(nameof(logRepository));
        }

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