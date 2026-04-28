using Domain;

namespace Services;

public interface ITeamService
{
    public void Add(string name, Confederation confederation, int fifaRanking);
}