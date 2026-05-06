namespace Domain;

public class StandingCalculator
{
    public List<Standing> CalculateStandings(Group group)
    {
        List<Standing> standings = new List<Standing>();

        foreach (Team team in group.TeamsInGroup)
        {
            standings.Add(new Standing(team));
        }
        
        foreach (Match match in group.MatchesInGroup)
        {
            Standing homeStanding = standings.First(s => s.TeamInGroup == match.HomeTeam);
            Standing awayStanding = standings.First(s => s.TeamInGroup == match.AwayTeam);
            homeStanding.AddMatchResult(match.HomeGoals.Value, match.AwayGoals.Value);
            awayStanding.AddMatchResult(match.AwayGoals.Value, match.HomeGoals.Value);
        }
        
        return standings
            .OrderByDescending(standing => standing.PointsInGroup)
            .ThenByDescending(standing => standing.GoalDifference)
            .ThenByDescending(standing => standing.GoalsFor)
            .ToList();
    }
}