namespace ServicesTests.TestHelpers;

public class ImportCsvBuilder
{
    public static string CreateValidCsvWith48Teams()
    {
        var lines = new List<string>
        {
            "Nombre,Confederación,RankingFIFA"
        };

        for (int i = 1; i <= 48; i++)
        {
            lines.Add($"Equipo{i},UEFA,{300 + i}");
        }

        return string.Join(Environment.NewLine, lines);
    }
}