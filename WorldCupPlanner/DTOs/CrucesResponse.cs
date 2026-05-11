namespace DTOs;

public class CruceResponse
{
    public string Name { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public int? HomeGoals { get; set; }
    public int? AwayGoals { get; set; }
    public string Phase { get; set; }
}