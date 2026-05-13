namespace Domain;

public class CsvParser
{
    public ParsedCsv Parse(string csvContent)
    {
        string[] lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        List<string> headers = lines[0]
            .Split(',')
            .Select(header => header.Trim())
            .ToList();

        var parsedCsv = new ParsedCsv(headers);

        for (int i = 1; i < lines.Length; i++)
        {
            ParsedCsvRow row = CreateRow(i + 1, headers, lines[i]);
            parsedCsv.AddRow(row);
        }

        return parsedCsv;
    }

    private ParsedCsvRow CreateRow(int rowNumber, List<string> headers, string line)
    {
        string[] values = line.Split(',');
        var rowValues = new Dictionary<string, string>();

        for (int i = 0; i < headers.Count; i++)
        {
            string value = i < values.Length ? values[i].Trim() : string.Empty;
            rowValues[headers[i]] = value;
        }

        return new ParsedCsvRow(rowNumber, rowValues);
    }
}