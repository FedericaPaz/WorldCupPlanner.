using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Calculators;

public class StandingCalculator
{
    public List<Standing> CalculateStandings(Group group)
    {
        if (group is null)
            throw new ArgumentNullException(nameof(group));

        List<Standing> standings = CreateStandingsForGroup(group);

        ApplyMatchResults(group, standings);

        return OrderStandings(standings);
    }

    private List<Standing> CreateStandingsForGroup(Group group)
    {
        List<Standing> standings = new List<Standing>();
        
        foreach (Team team in group.GetTeamsInGroup())  
        {
            standings.Add(new Standing(team));
        }

        return standings;
    }

    private void ApplyMatchResults(Group group, List<Standing> standings)
    {
        foreach (Match match in group.MatchesInGroup)
        {
            if (MatchHasNoResult(match))
                continue;

            ApplyMatchResult(match, standings);
        }
    }

    private bool MatchHasNoResult(Match match)
    {
        return !match.HomeGoals.HasValue || !match.AwayGoals.HasValue;
    }

    private void ApplyMatchResult(Match match, List<Standing> standings)
    {
        Standing homeStanding = FindStandingForTeam(standings, match.HomeTeam);
        Standing awayStanding = FindStandingForTeam(standings, match.AwayTeam);

        int homeGoals = match.HomeGoals!.Value;
        int awayGoals = match.AwayGoals!.Value;

        homeStanding.AddMatchResult(homeGoals, awayGoals);
        awayStanding.AddMatchResult(awayGoals, homeGoals);
    }

    private Standing FindStandingForTeam(List<Standing> standings, ITeam team)
    {
        return standings.First(s => s.TeamInGroup.Name == team.Name);
    }

    private List<Standing> OrderStandings(List<Standing> standings)
    {
        return standings
            .OrderByDescending(standing => standing.PointsInGroup)
            .ThenByDescending(standing => standing.GoalDifference)
            .ThenByDescending(standing => standing.GoalsFor)
            .ToList();
    }
}