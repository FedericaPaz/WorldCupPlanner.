using Domain.Entities;
using DTOs.Team;
using IRepository;
using IServices.TeamInterfaces;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamQueryService, ITeamEditService
{
    private readonly ITeamRepository _repository;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ICrucesRepository _crucesRepository;
    private readonly TournamentConfig _config;

    public TeamService(
        ITeamRepository repository,
        IFixtureRepository fixtureRepository,
        IMatchRepository matchRepository,
        ICrucesRepository crucesRepository,
        TournamentConfig config)
    {
        _repository = repository;
        _fixtureRepository = fixtureRepository;
        _matchRepository = matchRepository;
        _crucesRepository = crucesRepository;
        _config = config;
    }

    public void Add(TeamDTO request)
    {
        ValidateRequest(request);

        if (_repository.Get(request.Name) != null)
            throw new ArgumentException($"Ya existe un equipo con el nombre '{request.Name}'.");

        var confederation = new Confederation(request.ConfederationName);
        var currentCount = _repository.GetAll().Count(t => t.GetConfederationName() == confederation.Name);

        if (!confederation.CanAddTeam(currentCount))
            throw new InvalidOperationException($"La confederación '{confederation.Name}' alcanzó su límite de {confederation.MaxTeams} equipos.");

        _repository.Add(request.ToEntity());
    }

    public TeamDTO? Get(string name)
    {
        var team = _repository.Get(name);
        return team?.FromEntity();
    }

    public List<TeamDTO> GetAll()
    {
        return _repository.GetAll()
            .Select(t => t.FromEntity())
            .ToList();
    }

    public void Update(string currentName, TeamDTO request)
    {
        if (string.IsNullOrWhiteSpace(currentName))
            throw new ArgumentException("El nombre actual del equipo no puede estar vacío.");

        ValidateRequest(request);

        var existingTeam = _repository.Get(currentName)
                           ?? throw new InvalidOperationException($"No existe un equipo con el nombre '{currentName}'.");

        var newConf = new Confederation(request.ConfederationName);
        var oldConf = existingTeam.Confederation;

        if (!newConf.Equals(oldConf))
        {
            var currentCount = _repository.GetAll().Count(t => t.GetConfederationName() == newConf.Name);
            if (!newConf.CanAddTeam(currentCount))
                throw new InvalidOperationException($"La confederación '{newConf.Name}' alcanzó su límite de {newConf.MaxTeams} equipos.");
        }

        if (!currentName.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (_repository.Get(request.Name) is not null)
                throw new ArgumentException($"Ya existe un equipo con el nombre '{request.Name}'.");

            _repository.Rename(currentName, request.ToEntity());
            return;
        }

        _repository.Update(request.ToEntity());
    }

    public void Delete(string name)
    {
        var team = _repository.Get(name)
                   ?? throw new InvalidOperationException($"No existe un equipo con el nombre '{name}'.");

        _crucesRepository.RemoveCruces();
        _matchRepository.ClearMatches();

        if (_fixtureRepository.ContainsFixture())
            _fixtureRepository.RemoveFixture(_fixtureRepository.GetFixture());

        _repository.Delete(team);
    }

    public List<ConfederationDTO> GetAllConfederations()
    {
        return Confederation.GetAllowedNames()
            .Select(name => new ConfederationDTO
            {
                Name = name,
                MaxTeams = Confederation.GetMaxTeamsForName(name)
            })
            .ToList();
    }

    public void GenerateMissingTeams()
    {
        TeamGenerator.GenerateMissingTeams(_repository, _config);
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
        return _repository.GetAll()
            .Count(t => t.GetConfederationName() == confederationName);
    }
}