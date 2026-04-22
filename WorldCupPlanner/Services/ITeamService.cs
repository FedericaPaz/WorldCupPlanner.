using Domain;

namespace Services;

public interface ITeamService
{
    public void Create(string name, Confederation confederation, int fifaRanking);
}