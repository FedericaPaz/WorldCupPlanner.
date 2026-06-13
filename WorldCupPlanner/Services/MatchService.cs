using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Match;
using IRepository;
using IServices;
using Services.Mappers;
using Services.Simulation;

namespace Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IDateTimeMatch _clock;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly INotificationService _notificationService;
    private readonly IRankingService _rankingService;
    private readonly ISimulationEngineFactory _engineFactory;
    private readonly ISimulationService _simulationService; // <-- Agregado para soportar los métodos de simulación
    private readonly TournamentConfig _config;             // <-- Agregado para usar _config.SemillaSimulation

    public MatchService(
        IMatchRepository matchRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IDateTimeMatch clock,
        IFixtureRepository fixtureRepository,
        INotificationService notificationService,
        IRankingService rankingService,
        ISimulationEngineFactory engineFactory,         // <-- Cambiado de New() a Inyección
        ISimulationService simulationService,           // <-- Inyectado
        TournamentConfig config)                         // <-- Inyectado
    {
        _matchRepository = matchRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _clock = clock;
        _fixtureRepository = fixtureRepository;
        _notificationService = notificationService;
        _rankingService = rankingService;
        _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
        _simulationService = simulationService;
        _config = config;
    }

    public MatchResponse CreateMatch(MatchRequest request)
    {
        EnsureValidRequest(request);

        var stadium = GetStadiumOrThrow(request.stadiumName);
        var homeTeam = GetTeamOrThrow(request.homeTeamName, "local");
        var awayTeam = GetTeamOrThrow(request.awayTeamName, "visitante");

        var match = CreateMatchEntity(request, stadium, homeTeam, awayTeam);
        _matchRepository.AddMatch(match);

        return match.ToResponse();
    }

    public void SetMatchResult(int matchId, MatchResultRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var match = _matchRepository.GetMatchById(matchId);
        match.ClearAllIncidents();

        AddCardIncidents(match, request);
        match.UpdateMatchResult(request.homeGoals, request.awayGoals);

        _matchRepository.UpdateMatch(match);
        _rankingService.UpdateRankingsAfterMatch(match);

        _notificationService.NotifyJournalists(
            $"Se cargó el resultado de {match.HomeTeam.Name} vs {match.AwayTeam.Name}: {request.homeGoals}-{request.awayGoals}");
    }

    public MatchResponse GetMatchById(int matchId)
    {
        var match = _matchRepository.GetMatchById(matchId);
        return match.ToResponse();
    }

    public List<MatchResponse> GetAllMatches()
    {
        return _matchRepository
            .GetAllMatches()
            .Select(match => match.ToResponse())
            .ToList();
    }

    public void UpdateMatchDate(int matchId, DateTime date)
    {
        var match = _matchRepository.GetMatchById(matchId);
        match.UpdateDate(date);
    }

    public void UpdateMatchStadium(int matchId, string stadiumName)
    {
        var match = _matchRepository.GetMatchById(matchId);
        var stadium = GetStadiumOrThrow(stadiumName);
        match.UpdateStadium(stadium);
    }

    public void SimulateMatch(int matchId, int seed)
    {
        SimulateMatch(matchId, seed, _engineFactory.Create(null));
    }

    public void SimulateMatch(int matchId, int seed, ISimulationEngine engine)
    {
        var match = _matchRepository.GetMatchById(matchId);

        var (home, away) = _simulationService.SimulateMatch(match, seed, engine);

        var random = new Random(seed);

        var request = new MatchResultRequest
        {
            homeGoals = home,
            awayGoals = away,
            homeYellowCards = random.Next(0, 5),
            awayYellowCards = random.Next(0, 5),
            homeRedCards = random.Next(0, 2),
            awayRedCards = random.Next(0, 2)
        };

        SetMatchResult(matchId, request);
    }

    public void SimulateGroupStage()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        ISimulationEngine engine = _engineFactory.Create(fixture.SimulationEngineName);
        SimulateGroupStage(engine);
    }

    public void SimulateGroupStage(ISimulationEngine engine)
    {
        var fixture = _fixtureRepository.GetFixture();

        if (fixture == null)
            throw new InvalidOperationException("No hay fixture generado.");
        
        var allMatches = _matchRepository.GetAllMatches();
        foreach (var group in fixture.Groups)
        {
            var groupMatches = allMatches.Where(m => m.Group == group.GroupLetter).ToList();
            foreach (var match in groupMatches)
                group.LoadMatchFromRepository(match);
        }
        
        foreach (var group in fixture.Groups)
        {
            foreach (var match in group.MatchesInGroup)
            {
                Console.WriteLine($"Grupo {group.GroupLetter} | Match {match.IdMatch} | " +
                                  $"{match.HomeTeam.Name} vs {match.AwayTeam.Name} | " +
                                  $"Goals: {match.HomeGoals?.ToString() ?? "null"}-{match.AwayGoals?.ToString() ?? "null"} | " +
                                  $"Pendiente: {!match.HomeGoals.HasValue && !match.AwayGoals.HasValue}");
            }
        }
        
        // Solo le pasamos el engine, tal como pide tu interfaz
        _simulationService.SimulateGroupStage(engine);
    }

    public bool MatchesHaveBeenPlayed()
    {
        return _matchRepository.GetAllMatches()
            .Any(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue);
    }

    private static void EnsureValidRequest(MatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.group))
            throw new ArgumentException("El grupo es obligatorio.");
    }

    private Stadium GetStadiumOrThrow(string stadiumName)
    {
        var stadium = _stadiumRepository.GetStadium(stadiumName);
        if (stadium == null)
            throw new KeyNotFoundException("No existe un estadio con ese nombre.");
        return stadium;
    }

    private Team GetTeamOrThrow(string teamName, string teamType)
    {
        var team = _teamRepository.Get(teamName);
        if (team == null)
            throw new KeyNotFoundException($"No existe el equipo {teamType}.");
        return team;
    }

    private Match CreateMatchEntity(MatchRequest request, Stadium stadium, Team homeTeam, Team awayTeam)
    {
        int id = _matchRepository.GetNextId();
        return new Match(id, request.dateMatch, stadium, homeTeam, awayTeam, request.group[0], _clock);
    }

    private static void AddCardIncidents(Match match, MatchResultRequest request)
    {
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.HomeTeam, request.homeYellowCards);
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.AwayTeam, request.awayYellowCards);
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.HomeTeam, request.homeRedCards);
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.AwayTeam, request.awayRedCards);
    }

    private static void AddIncidentIfNeeded(Match match, IncidentType type, Team team, int quantity)
    {
        if (quantity > 0)
            match.AddIncident(new MatchIncident(type, team, quantity));
    }
}