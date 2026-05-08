namespace Domain;

public class ParsedCsv
{
    private readonly List<ParsedCsvRow> _rows = new();

    public IReadOnlyList<ParsedCsvRow> Rows
    {
        get { return _rows; }
    }

    public void AddRow(ParsedCsvRow row)
    {
        _rows.Add(row);
    }
}