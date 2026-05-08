namespace DTOs;

public class FixtureResponse
{
    public List<GroupResponse> Groups { get; set; } = new();
    public string CurrentPhase { get; set; } = string.Empty;
}