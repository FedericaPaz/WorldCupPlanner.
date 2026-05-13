namespace DTOs;

public class ImportResponse
{
    public bool Success { get; set; }
    public int ImportedTeamsCount { get; set; }
    public List<ImportErrorResponse> Errors { get; set; } = new();
}