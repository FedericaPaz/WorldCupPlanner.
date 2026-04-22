using Domain;
using IRepository;
using Repository;

namespace Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _repository;

    public TeamService(ITeamRepository repository)
    {
        _repository = repository;
    }

    public void Create(string name, Confederation confederation, int fifaRanking)
    {
        var team1 = _repository.Get(name);
        if(team1 != null)
        {
            throw new ArgumentException($"A team with the name '{name}' already exists.");
        }
        
        var team = new Team(name, confederation, fifaRanking);
        _repository.Add(team);
    }
    
    
}