namespace Domain;

public class ParsedCsvRow
{
    private readonly Dictionary<string, string> _values;
    private int _rowNumber;

    public int RowNumber
    {
        get => _rowNumber;
        set => _rowNumber = value;
    }

    public ParsedCsvRow(int rowNumber, Dictionary<string, string> values)
    {
        RowNumber = rowNumber;
        _values = values;
    }
    
    public string GetValue(string columnName)
    {
        return _values[columnName];
    }
}