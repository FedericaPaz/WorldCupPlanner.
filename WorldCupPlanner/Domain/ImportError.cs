namespace Domain;

public class ImportError
{
    private int _rowNumber;
    private string _message;

    public int RowNumber
    {
        get { return _rowNumber; }
        set => _rowNumber = value;
    }

    public string Message
    {
        get { return _message; }
        set => _message = value;
    }

    public ImportError(int rowNumber, string message)
    {
        RowNumber = rowNumber;
        Message = message;
    }
}