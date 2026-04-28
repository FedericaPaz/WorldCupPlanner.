using Domain;
using DTOs;
using IRepository;

namespace Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _repository;

    public TeamService(ITeamRepository repository)
    {
        _repository = repository;
    }

    public void Add(TeamDTOs.CreateTeamDTO request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
            
        if(string.IsNullOrEmpty(request.ConfederationName))
            throw new ArgumentNullException(nameof(request.ConfederationName));
        
        var existingTeam = _repository.Get(request.Name);
        if(existingTeam != null)
            throw new ArgumentException($"Team with the name '{request.Name}' already exists.");
        
        var confederation = new Confederation(request.ConfederationName);
        
        var count = _repository.GetAll().Count(t => t.Confederation.Name == confederation.Name);
        if (count >= GetLimit(confederation.Name))
            throw new Exception($"The confederation '{confederation.Name}' has reached its team limit.");
        
        var team = new Team(request.Name, confederation, request.FifaRanking);
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
    
    public TeamDTOs.TeamResponseDTO? Get(string name)
    {
        var team = _repository.Get(name);
        return team == null ? null : TeamDTOs.TeamResponseDTO.FromEntity(team);
    }

    public List<TeamDTOs.TeamResponseDTO> GetAll()
    {
        return _repository.GetAll().Select(TeamDTOs.TeamResponseDTO.FromEntity).ToList();
    }
    
    /*public void Update(Team team)
    {
        _repository.Update(team);
    }*/

    public void Delete(string name)
    {
        var team = _repository.Get(name);
        if (team is null)
            throw new InvalidOperationException($"Team with the name '{name}' does not exist.");
            
        _repository.Delete(team);
    }
}