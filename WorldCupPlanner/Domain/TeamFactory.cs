namespace Domain;

public class TeamFactory
{
    public Team CreateTeam(string name, string confederationName, string rankingValue)
    {
        return new Team(name, new Confederation(confederationName), int.Parse(rankingValue));
    }
}