using Domain;

namespace DomainTests.Stubs;

public class TeamImportValidatorStub : ITeamImportValidator
{
    public List<ImportError> ValidateHeaders(IReadOnlyList<string> headers)
    {
        return new List<ImportError>();
    }

    public List<ImportError> ValidateRow(ParsedCsvRow row, HashSet<string> importedNames)
    {
        return new List<ImportError>();
    }
}