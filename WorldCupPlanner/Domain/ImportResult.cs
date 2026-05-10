namespace Domain;

public class ImportResult
{
    private readonly List<Team> _teams = new();
    private readonly List<ImportError> _errors = new();

    public IReadOnlyList<Team> Teams
    {
        get { return _teams; }
    }

    public IReadOnlyList<ImportError> Errors
    {
        get { return _errors; }
    }

    public bool Success
    {
        get { return _errors.Count == 0; }
    }
}