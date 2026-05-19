namespace Domain.Import;

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
        if (!_values.ContainsKey(columnName))
            throw new ArgumentException($"La columna '{columnName}' no existe.");
        return _values[columnName];
    }
}