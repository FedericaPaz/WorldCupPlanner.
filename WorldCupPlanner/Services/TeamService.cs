// TODO: Mover esta logica a la entidad Confederation (getLimit)
// Ver: Domain/Confederation.cs - agregar método estático GetMaxTeams(string name)
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _repository;

    public TeamService(ITeamRepository repository)
    {
        _repository = repository;
    }

    public void Add(TeamDTO request)
    {
        ValidateRequest(request);
        var existingTeam = _repository.Get(request.Name);
        if (existingTeam != null)
            throw new ArgumentException($"Team with the name '{request.Name}' already exists.");

        var confederationName = request.ConfederationName!;
        var count = _repository.GetAll().Count(t => t.Confederation.Name == confederationName);
        if (count >= GetLimit(confederationName))
            throw new InvalidOperationException($"The confederation '{confederationName}' has reached its team limit.");

        var team = request.ToEntity();
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
            _ => throw new InvalidOperationException("Invalid confederation name.")
        };
    }
    
    public TeamDTO? Get(string name)
    {
        var team = _repository.Get(name);
        return team?.FromEntity();
    }

    public List<TeamDTO> GetAll()
    {
        return _repository.GetAll().Select(t => t.FromEntity()).ToList();
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
    
    private static void ValidateRequest(TeamDTO request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(request.ConfederationName))
            throw new ArgumentNullException(nameof(request.ConfederationName));
    }
}