namespace DTOs.Log;

public class LogEntryDTO
{
    public DateTime Timestamp { get; set; }
    public string UserEmail { get; set; }
    public string Category { get; set; }
    public string Action { get; set; }
    public string Detail { get; set; }
}