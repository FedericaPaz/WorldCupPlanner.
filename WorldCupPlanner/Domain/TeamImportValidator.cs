namespace Domain;

public class TeamImportValidator : ITeamImportValidator
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
        string confederationName = row.GetValue(ConfederationHeader);
        string rankingValue = row.GetValue(RankingHeader);

        if (importedNames.Contains(name))
            errors.Add(new ImportError(row.RowNumber, "El nombre del equipo está duplicado."));

        if (!Confederation.IsValidName(confederationName))
            errors.Add(new ImportError(row.RowNumber, "La confederación no es válida."));

        if (!int.TryParse(rankingValue, out int ranking) || ranking < Team.MinRanking || ranking > Team.MaxRanking)
            errors.Add(new ImportError(row.RowNumber, $"El ranking FIFA debe estar entre {Team.MinRanking} y {Team.MaxRanking}."));

        return errors;
    }
}