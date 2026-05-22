using Domain.Entities;
using DTOs.Team;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class TeamService : ITeamService
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

        var existingTeam = _repository.Get(request.Name);
        if (existingTeam != null)
            throw new ArgumentException($"Ya existe un equipo con el nombre '{request.Name}'.");

        //ValidateRankingIsNotUsed(request.FifaRanking);

        var confederationName = request.ConfederationName;
        var normalizedConf = confederationName.Trim().ToUpperInvariant();
        var maxTeams = Confederation.GetMaxTeamsForName(normalizedConf);

        if (maxTeams == 0)
            throw new ArgumentException($"La confederación '{confederationName}' no es válida.");

        var currentCount = _repository.GetAll()
            .Count(t => t.Confederation.Name == normalizedConf);

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
        return _repository.GetAll()
            .Select(t => t.FromEntity())
            .ToList();
    }

    public void Update(string currentName, TeamDTO request)
    {
        if (string.IsNullOrWhiteSpace(currentName))
            throw new ArgumentException("El nombre actual del equipo no puede estar vacío.");

        ValidateRequest(request);

        var existingTeam = _repository.Get(currentName);
        if (existingTeam is null)
            throw new InvalidOperationException($"No existe un equipo con el nombre '{currentName}'.");

        var newConf = request.ConfederationName.Trim().ToUpperInvariant();
        var oldConf = existingTeam.Confederation.Name.Trim().ToUpperInvariant();

        if (!newConf.Equals(oldConf, StringComparison.OrdinalIgnoreCase))
        {
            var maxTeams = Confederation.GetMaxTeamsForName(newConf);
            if (maxTeams == 0)
                throw new ArgumentException($"La confederación '{request.ConfederationName}' no es válida.");

            var currentCount = _repository.GetAll()
                .Count(t => t.Confederation.Name == newConf);

            if (currentCount >= maxTeams)
                throw new InvalidOperationException($"La confederación '{request.ConfederationName}' alcanzó su límite de {maxTeams} equipos.");
        }

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
        if (_fixtureRepository.ContainsFixture())
        {
            _fixtureRepository.RemoveFixture(_fixtureRepository.GetFixture());
            _matchRepository.ClearMatches();
            _crucesRepository.RemoveCruces();
        }
        
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
        
        var random = new Random(_config.SemillaCompletar);
        const int RankingMin = 300;
        const int RankingMax = 2500;
        foreach (var conf in confederations)
        {
            int confIndex = 1;
            for (int i = 0; i < conf.limit; i++)
            {
                string name = conf.name + "_" + confIndex.ToString("D2");
                while (_repository.Get(name) != null)
                {
                    confIndex++;
                    name = conf.name + "_" + confIndex.ToString("D2");
                }
                confIndex++;
                
                int ranking = random.Next(RankingMin, RankingMax + 1);
                _repository.Add(new Team(name, new Confederation(conf.name), ranking));
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

    /*private void ValidateRankingIsNotUsed(int fifaRanking)
    {
        var rankingAlreadyExists = _repository.GetAll()
            .Any(t => t.FifaRanking == fifaRanking);

        if (rankingAlreadyExists)
            throw new ArgumentException($"Ya existe un equipo con el ranking FIFA {fifaRanking}.");
    }*/

   /* private void ValidateRankingIsNotUsedByAnotherTeam(int fifaRanking, string currentName)
    {
        var todos = _repository.GetAll();
        Console.WriteLine($"Validando ranking {fifaRanking} para {currentName}");
        foreach (var t in todos)
            Console.WriteLine($"  - {t.Name}: {t.FifaRanking}");

        var rankingBelongsToAnotherTeam = todos.Any(t =>
            t.FifaRanking == fifaRanking &&
            !t.Name.Equals(currentName, StringComparison.OrdinalIgnoreCase));

        Console.WriteLine($"  Resultado: {rankingBelongsToAnotherTeam}");

        if (rankingBelongsToAnotherTeam)
            throw new ArgumentException($"Ya existe otro equipo con el ranking FIFA {fifaRanking}.");
    }*/

    private int CountConfederation(string confederationName)
    {
        return _repository.GetAll()
            .Count(t => t.Confederation.Name == confederationName);
    }
}