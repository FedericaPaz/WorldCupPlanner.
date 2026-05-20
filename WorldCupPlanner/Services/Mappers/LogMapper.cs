using Domain.Entities;
using DTOs.Logs;

namespace Services.Mappers;

public static class LogMapper
{
    public static LogEntryDTO ToDTO(LogEntry logEntry)
    {
        return new LogEntryDTO
        {
            Timestamp = logEntry.Timestamp,
            UserEmail = logEntry.UserEmail,
            Category = logEntry.Category,
            Action = logEntry.Action,
            Detail = logEntry.Detail
        };
    }
}