namespace Domain;

public class TeamFactory : ITeamFactory
{
    public Team CreateTeam(string name, string confederationName, string rankingValue)
    {
        if (!int.TryParse(rankingValue, out int ranking))
            throw new ArgumentException("El ranking FIFA debe ser de tipo numérico.");
        return new Team(name, new Confederation(confederationName), int.Parse(rankingValue));
    }
}