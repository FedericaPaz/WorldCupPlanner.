namespace Domain;

public class LogEntry
{
    private DateTime _timestamp;
    private string _userEmail;
    private string _category;
    private string _action;
    private string _detail;

    public DateTime Timestamp
    {
        get { return _timestamp; }
        private set { _timestamp = value; }
    }

    public string UserEmail
    {
        get { return _userEmail; }
        private set { _userEmail = value; }
    }

    public string Category
    {
        get { return _category; }
        private set { _category = value; }
    }

    public string Action
    {
        get { return _action; }
        private set { _action = value; }
    }

    public string Detail
    {
        get { return _detail; }
        private set { _detail = value; }
    }

    public LogEntry(string userEmail, string category, string action, string detail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("El usuario del log es requerido.", nameof(userEmail));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("La categoría del log es requerida.", nameof(category));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("La acción del log es requerida.", nameof(action));

        if (string.IsNullOrWhiteSpace(detail))
            throw new ArgumentException("El detalle del log es requerido.", nameof(detail));

        Timestamp = DateTime.UtcNow;
        UserEmail = userEmail;
        Category = category;
        Action = action;
        Detail = detail;
    }
}