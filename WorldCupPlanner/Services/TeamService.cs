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

        var confederationName = request.ConfederationName;
        var normalizedConf = confederationName.Trim().ToUpperInvariant();
        var maxTeams = Confederation.GetMaxTeamsForName(normalizedConf);

        if (maxTeams == 0)
            throw new ArgumentException($"Invalid confederation name: {confederationName}");

        var currentCount = _repository.GetAll().Count(t => t.Confederation.Name == normalizedConf);

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

    public void GenerateMissingTeams()
    {
        int missing = 48 - (_repository.GetAll().Count());

        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7), ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };
    
        int teamIndex = 1;
        int rankingBase = 500;
        foreach (var (name, limit) in confederations)
        {
            int teamsToAdd = Math.Min(limit, missing - teamIndex + 1);
            for (int i = 1; i <= teamsToAdd && teamIndex <= missing; i++)
            {
                var team = new Team($"Team_{teamIndex}", new Confederation(name), rankingBase + teamIndex);
                _repository.Add(team);
                teamIndex++;
            }
        }
    }
}