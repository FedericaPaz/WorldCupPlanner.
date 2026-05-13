namespace Domain;

public class ParsedCsv
{
    private readonly List<string> _headers;
    private readonly List<ParsedCsvRow> _rows = new();

    public IReadOnlyList<string> Headers
    {
        get { return _headers; }
    }

    public IReadOnlyList<ParsedCsvRow> Rows
    {
        get { return _rows; }
    }

    public ParsedCsv()
    {
        _headers = new List<string>();
    }

    public ParsedCsv(List<string> headers)
    {
        _headers = headers;
    }

    public void AddRow(ParsedCsvRow row)
    {
        _rows.Add(row);
    }
}