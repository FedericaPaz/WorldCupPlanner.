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
    
    public List<ImportError> ValidateRow(ParsedCsvRow row, HashSet<string> importedNames)
    {
        List<ImportError> errors = new List<ImportError>();
        string name = row.GetValue(NameHeader);
        if (importedNames.Contains(name))
            errors.Add(new ImportError(row.RowNumber, "El nombre del equipo está duplicado."));
        return errors;
    }
}