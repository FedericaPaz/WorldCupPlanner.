using DTOs.Match;
using DTOs.Team;

namespace DTOs.Fixture;

public class GroupResponse
{
    public char GroupLetter { get; set; }
    public List<TeamDTO> Teams { get; set; } = new();
    public List<MatchResponse> Matches { get; set; } = new();
}