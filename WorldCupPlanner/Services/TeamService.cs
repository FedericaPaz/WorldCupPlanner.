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
        if (confederation is null)
            throw new ArgumentNullException(nameof(confederation));
            
        var team1 = _repository.Get(name);
        if(team1 != null)
        {
            throw new ArgumentException($"A team with the name '{name}' already exists.");
        }
        
        var count = _repository.GetAll().Count(t => t.Confederation.Name == confederation.Name);
        if (count >= GetLimit(confederation.Name))
            throw new Exception($"The confederation '{confederation.Name}' has reached its team limit.");
        
        var team = new Team(name, confederation, fifaRanking);
        _repository.Add(team);
    }

    private int GetLimit(string confederationName)
    {
        return confederationName switch
        {
            "UEFA" => 16,
            "CONMEBOL" => 7,
            "CONCACAF" => 7,
            "OFC" => 1,
            "CAF" => 9,
            "AFC" => 8,
            _ => throw new Exception("Invalid confederation name.")
        };
    }
    
    
    
    
}