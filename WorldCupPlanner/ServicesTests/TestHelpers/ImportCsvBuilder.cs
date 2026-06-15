namespace ServicesTests.TestHelpers;

public class ImportCsvBuilder
{
    public static string CreateValidCsvWith48Teams()
    {
        var lines = new List<string> { "Nombre,Confederación,RankingFIFA" };
    
        var confederaciones = new (string nombre, int cupo)[]
        {
            ("UEFA", 16),
            ("CONMEBOL", 7),
            ("CONCACAF", 7),
            ("CAF", 9),
            ("AFC", 8),
            ("OFC", 1)
        };

        int ranking = 301;
        int equipoNum = 1;
        foreach (var (nombre, cupo) in confederaciones)
            for (int i = 0; i < cupo; i++)
                lines.Add($"Equipo{equipoNum++},{nombre},{ranking++}");

        return string.Join(Environment.NewLine, lines);
    }
}