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
            throw new ArgumentException($"Ya existe un equipo con el nombre '{request.Name}'.");

        var existingRanking = _repository.GetAll()
            .Any(t => t.FifaRanking == request.FifaRanking);

        if (existingRanking)
            throw new ArgumentException($"Ya existe un equipo con el ranking FIFA {request.FifaRanking}.");

        var confederationName = request.ConfederationName;
        var normalizedConf = confederationName.Trim().ToUpperInvariant();
        var maxTeams = Confederation.GetMaxTeamsForName(normalizedConf);

        if (maxTeams == 0)
            throw new ArgumentException($"La confederación '{confederationName}' no es válida.");

        var currentCount = _repository.GetAll().Count(t => t.Confederation.Name == normalizedConf);

        if (currentCount >= maxTeams)
            throw new InvalidOperationException($"La confederación '{confederationName}' alcanzó su límite de {maxTeams} equipos.");

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

    public void Update(string currentName, TeamDTO request)
    {
        if (string.IsNullOrWhiteSpace(currentName))
            throw new ArgumentException("El nombre actual del equipo no puede estar vacío.");

        ValidateRequest(request);

        var existingTeam = _repository.Get(currentName);

        if (existingTeam is null)
            throw new InvalidOperationException($"No existe un equipo con el nombre '{currentName}'.");

        if (!currentName.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var teamWithNewName = _repository.Get(request.Name);

            if (teamWithNewName is not null)
                throw new ArgumentException($"Ya existe un equipo con el nombre '{request.Name}'.");

            _repository.Delete(existingTeam);
            _repository.Add(request.ToEntity());
            return;
        }

        _repository.Update(request.ToEntity());
    }

    public void Delete(string name)
    {
        var team = _repository.Get(name);
        if (team is null)
            throw new InvalidOperationException($"No existe un equipo con el nombre '{name}'.");

        _repository.Delete(team);
    }

    public List<ConfederationDTO> GetAllConfederations()
    {
        List<ConfederationDTO> result = new();

        foreach (string name in Confederation.GetAllowedNames())
        {
            result.Add(new ConfederationDTO
            {
                Name = name,
                MaxTeams = Confederation.GetMaxTeamsForName(name)
            });
        }

        return result;
    }

    public void GenerateMissingTeams()
    {
        int missing = 48 - _repository.GetAll().Count;
        int uefa = 16 - CountConfederation("UEFA");
        int concacaf = 7 - CountConfederation("CONCACAF");
        int conmebol = 7 - CountConfederation("CONMEBOL");
        int caf = 9 - CountConfederation("CAF");
        int afc = 8 - CountConfederation("AFC");
        int ofc = 1 - CountConfederation("OFC");

        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", uefa),
            ("CONMEBOL", conmebol),
            ("CONCACAF", concacaf),
            ("CAF", caf),
            ("AFC", afc),
            ("OFC", ofc)
        };

        int teamIndex = 1;
        int rankingBase = 500;

        foreach (var conf in confederations)
        {
            int added = 0;

            while (added < conf.limit && teamIndex <= missing)
            {
                var team = new Team($"Team_{teamIndex}", new Confederation(conf.name), rankingBase + teamIndex);
                _repository.Add(team);
                teamIndex++;
                added++;
            }
        }
    }

    private static void ValidateRequest(TeamDTO request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("El nombre del equipo no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(request.ConfederationName))
            throw new ArgumentException("La confederación del equipo no puede estar vacía.");
    }

    private int CountConfederation(string confederationName)
    {
        return _repository.GetAll().Count(t => t.Confederation.Name == confederationName);
    }
}