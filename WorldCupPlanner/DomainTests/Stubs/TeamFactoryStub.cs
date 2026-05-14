using Domain;

namespace DomainTests.Stubs;

public class TeamFactoryStub : ITeamFactory
{
    public Team CreateTeam(string name, string confederationName, string rankingValue)
    {
        int ranking = 0;
        int.TryParse(rankingValue, out ranking);

        return new Team(
            name,
            new Confederation(confederationName),
            ranking
        );
    }
}