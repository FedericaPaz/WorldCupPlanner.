using DTOs.Logs;

namespace IServices;

public interface ILogService
{
    void Register(string userEmail, string category, string action, string detail);
    List<LogEntryDTO> GetAll();
}