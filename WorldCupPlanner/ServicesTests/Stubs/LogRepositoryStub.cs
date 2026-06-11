using Domain.Entities;
using IRepository;

namespace ServicesTests.Stubs;

public class LogRepositoryStub : ILogRepository
{
    public List<LogEntry> Logs { get; } = new();

    public void Add(LogEntry logEntry)
    {
        Logs.Add(logEntry);
    }

    public List<LogEntry> GetAll()
    {
        return Logs;
    }
}