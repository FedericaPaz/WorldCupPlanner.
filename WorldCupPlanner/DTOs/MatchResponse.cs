namespace DTOs;

public class MatchResponse
{
    public int idMatch { get; set; }
    public DateTime dateMatch { get; set; }
    public string stadiumName { get; set; } = string.Empty;
    public string homeTeamName { get; set; } = string.Empty;
    public string awayTeamName { get; set; } = string.Empty;
    public char group { get; set; }
    public int? homeGoals { get; set; }
    public int? awayGoals { get; set; }
    public string? winnerName { get; set; }
}