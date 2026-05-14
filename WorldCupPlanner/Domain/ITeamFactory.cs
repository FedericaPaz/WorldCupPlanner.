namespace Domain;

public interface ITeamFactory
{
    public Team CreateTeam(string name, string confederationName, string rankingValue);
}