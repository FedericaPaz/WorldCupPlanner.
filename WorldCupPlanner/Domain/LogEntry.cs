namespace Domain;

public class LogEntry
{
    public DateTime Timestamp { get; private set; }
    public string UserEmail { get; private set; }
    public string Category { get; private set; }
    public string Action { get; private set; }
    public string Detail { get; private set; }

    public LogEntry(string userEmail, string category, string action, string detail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            throw new ArgumentException("El usuario del log es requerido.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("La categoría del log es requerida.");
        }

        Timestamp = DateTime.UtcNow;
        UserEmail = userEmail;
        Category = category;
        Action = action;
        Detail = detail;
    }
}