namespace Domain;

public class TeamImportValidator
{
    private const string NameHeader = "Nombre";
    private const string ConfederationHeader = "Confederación";
    private const string RankingHeader = "RankingFIFA";

    public List<ImportError> ValidateHeaders(IReadOnlyList<string> headers)
    {
        List<ImportError> errors = new List<ImportError>();
        if (!headers.Contains(NameHeader) || !headers.Contains(ConfederationHeader) || !headers.Contains(RankingHeader))
        {
            errors.Add(new ImportError(1, "El encabezado del CSV no es válido."));
        }
        return errors;
    }
}