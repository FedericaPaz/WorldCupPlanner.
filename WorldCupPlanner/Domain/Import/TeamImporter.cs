using Domain.Entities;
using Domain.Import.Interfaces;


namespace Domain.Import;

public class TeamImporter
{
    private readonly ICsvParser _parser;
    private readonly ITeamImportValidator _validator;
    private readonly ITeamFactory _factory;

    public TeamImporter(ICsvParser parser, ITeamImportValidator validator, ITeamFactory factory)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public ImportResult Import(string csvContent)
    {
        ImportResult result = new ImportResult();
        ParsedCsv parsedCsv = _parser.Parse(csvContent);
        List<ImportError> headerErrors = _validator.ValidateHeaders(parsedCsv.Headers);

        foreach (ImportError error in headerErrors)
        {
            result.AddError(error);
        }

        if (headerErrors.Count > 0)
        {
            return result;
        }

        HashSet<string> importedNames = new HashSet<string>();

        foreach (ParsedCsvRow row in parsedCsv.Rows)
        {
            List<ImportError> rowErrors = _validator.ValidateRow(row, importedNames);

            foreach (ImportError error in rowErrors)
            {
                result.AddError(error);
            }

            if (rowErrors.Count > 0)
            {
                continue;
            }
            
            Team team = _factory.CreateTeam(
                row.GetValue("Nombre"),
                row.GetValue("Confederación"),
                row.GetValue("RankingFIFA"),
                row.GetValue("Bandera"));

            importedNames.Add(team.Name);
            result.AddTeam(team);
        }

        return result;
    }
}