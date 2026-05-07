using Domain;

namespace IRepository;

public interface ILogRepository
{
    void Add(LogEntry logEntry);
    List<LogEntry> GetAll();
}