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

        return standings;
    }
}