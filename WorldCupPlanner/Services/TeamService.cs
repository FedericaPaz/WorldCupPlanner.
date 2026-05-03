using Domain;
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
        var maxTeams = Confederation.GetMaxTeamsForName(confederationName);

        if (maxTeams == 0)
            throw new ArgumentException($"Invalid confederation name: {confederationName}");

        var currentCount = _repository.GetAll().Count(t => t.Confederation.Name == confederationName);

        if (currentCount >= maxTeams)
            throw new InvalidOperationException($"The confederation '{confederationName}' has reached its limit of {maxTeams} teams.");

        var team = request.ToEntity();
        _repository.Add(team);
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
    
    public List<ConfederationDTO> GetAllConfederations()
    {
        List<ConfederationDTO> result = new List<ConfederationDTO>();
        foreach (string name in Confederation.GetAllowedNames())
        {
            result.Add(new ConfederationDTO {
                Name = name,
                MaxTeams = Confederation.GetMaxTeamsForName(name)
            });
        }
        return result;
    }
}