using Domain.Entities;
using Domain.Import.Interfaces;

namespace Domain.Import;

public class TeamImporter
{
    private readonly ICsvParser _parser;
    private readonly ITeamImportValidator _validator;
    private readonly ITeamFactory _factory;

    public TeamImporter(ICsvParser parser, ITeamImportValidator validatoR, ITeamFactory factory)
    {
        if(parser == null)
            throw new ArgumentNullException(nameof(parser));
        if(validatoR == null)
            throw new ArgumentNullException(nameof(validatoR));
        if(factory == null)
            throw new ArgumentNullException(nameof(factory));
        
        _parser = parser;
        _validator = validatoR;
        _factory = factory;
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
                row.GetValue("RankingFIFA"));

            importedNames.Add(team.Name);
            result.AddTeam(team);
        }

        return result;
    }
}