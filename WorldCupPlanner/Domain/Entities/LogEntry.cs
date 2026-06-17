namespace Domain.Entities;

public class LogEntry
{
    private DateTime _timestamp;
    private string _userEmail = string.Empty;
    private string _category = string.Empty;
    private string _action = string.Empty;
    private string _detail = string.Empty;

    public int Id { get; }
    
    public DateTime Timestamp
    {
        get { return _timestamp; }
        private set { _timestamp = value; }
    }

    public string UserEmail
    {
        get { return _userEmail; }
        private set
        {
            EnsureRequiredText(value, "El usuario del log es requerido.", nameof(UserEmail));
            _userEmail = value.Trim();
        }
    }

    public string Category
    {
        get { return _category; }
        private set
        {
            EnsureRequiredText(value, "La categoría del log es requerida.", nameof(Category));
            _category = value.Trim();
        }
    }

    public string Action
    {
        get { return _action; }
        private set
        {
            EnsureRequiredText(value, "La acción del log es requerida.", nameof(Action));
            _action = value.Trim();
        }
    }

    public string Detail
    {
        get { return _detail; }
        private set
        {
            EnsureRequiredText(value, "El detalle del log es requerido.", nameof(Detail));
            _detail = value.Trim();
        }
    }

    private LogEntry()
    {
        
    }

    public LogEntry(string userEmail, string category, string action, string detail)
    {
        Timestamp = DateTime.UtcNow;
        UserEmail = userEmail;
        Category = category;
        Action = action;
        Detail = detail;
    }

    private static void EnsureRequiredText(string value, string errorMessage, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(errorMessage, parameterName);
        }
    }
}