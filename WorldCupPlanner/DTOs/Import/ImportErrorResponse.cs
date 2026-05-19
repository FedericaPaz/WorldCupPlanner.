namespace DTOs.Import;

public class ImportErrorResponse
{
    public int RowNumber { get; set; }
    public string Message { get; set; } = string.Empty;
}