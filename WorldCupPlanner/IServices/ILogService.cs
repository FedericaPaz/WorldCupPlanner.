using Domain;

namespace IServices;

public interface ILogService
{
    void Register(string userEmail, string category, string action, string detail);
    List<LogEntry> GetAll();
}