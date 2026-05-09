namespace Domain;

public class TeamImportValidator
{
    private const string NameHeader = "Nombre";
    private const string ConfederationHeader = "Confederación";
    private const string RankingHeader = "RankingFIFA";

    public List<ImportError> ValidateHeaders(IReadOnlyList<string> headers)
    {
        return new List<ImportError>();
    }
}