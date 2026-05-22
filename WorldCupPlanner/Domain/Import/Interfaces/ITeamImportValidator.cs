namespace Domain.Import.Interfaces;

public interface ITeamImportValidator
{
    public List<ImportError> ValidateHeaders(IReadOnlyList<string> headers);
    public List<ImportError> ValidateRow(ParsedCsvRow row, HashSet<string> importedNames);
}