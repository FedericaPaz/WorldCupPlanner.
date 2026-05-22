using Domain.Entities;

namespace IRepository;

public interface ILogRepository
{
    void Add(LogEntry logEntry);
    List<LogEntry> GetAll();
}