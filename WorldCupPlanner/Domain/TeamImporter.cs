namespace Domain;

public class TeamImporter
{
    private readonly CsvParser _parser = new();
    private readonly TeamImportValidator _validator = new();
    private readonly TeamFactory _factory = new();

    public ImportResult Import(string csvContent)
    {
        ImportResult result = new ImportResult();

        ParsedCsv parsedCsv = _parser.Parse(csvContent);

        foreach (ParsedCsvRow row in parsedCsv.Rows)
        {
            Team team = _factory.CreateTeam(
                row.GetValue("Nombre"),
                row.GetValue("Confederación"),
                row.GetValue("RankingFIFA"));

            result.AddTeam(team);
        }

        return result;
    }
}