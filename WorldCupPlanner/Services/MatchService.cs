using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Match;
using IRepository;
using IServices.MatchInterfaces;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchQueryService, IMatchEditService, IMatchCreateService, ISimulationService
{
    private const int MaxYellowCards = 4;
    private const int MaxRedCards = 1;

    private readonly IMatchRepository _matchRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IDateTimeMatch _clock;
    private readonly INotificationService _notificationService;
    private readonly ILogService _logService;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly TournamentConfig _config;

    public string LastUsedEngineName { get; private set; } = string.Empty;

    public MatchService(
        IMatchRepository matchRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IDateTimeMatch clock,
        INotificationService notificationService,
        ILogService logService,
        IFixtureRepository fixtureRepository,
        TournamentConfig config)
    {
        _matchRepository = matchRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _clock = clock;
        _notificationService = notificationService;
        _logService = logService;
        _fixtureRepository = fixtureRepository;
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

        bool alreadyHadResult = match.HomeGoals.HasValue && match.AwayGoals.HasValue;
        if (alreadyHadResult)
            RevertRankingsAfterMatch(match);

        if (match.Group == 'N' && request.homeGoals == request.awayGoals)
            throw new ArgumentException("Los partidos eliminatorios no pueden terminar en empate.");

        match.ClearAllIncidents();
        AddCardIncidents(match, request);
        PersistMatchResult(match, request.homeGoals, request.awayGoals);

        _notificationService.NotifyJournalists(
            $"Se cargó el resultado de {match.GetHomeTeamName()} vs {match.GetAwayTeamName()}: {request.homeGoals}-{request.awayGoals}");
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

    public MatchResponse GetMatchById(int matchId)
    {
        var match = _matchRepository.GetMatchById(matchId);
        return match.ToResponse();
    }

    public List<MatchResponse> GetAllMatches()
    {
        return _matchRepository
            .GetAllMatches()
            .Select(m => m.ToResponse())
            .ToList();
    }

    public bool MatchesHaveBeenPlayed()
    {
        return _matchRepository.GetAllMatches()
            .Any(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue);
    }

    public void SimulateMatch(int matchId, int seed)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        SimulateMatchInternal(matchId, seed, fixture.CreateEngine());
        _notificationService.NotifyJournalists("Se simuló el resultado de un partido.");
    }

    public void SimulateGroupStage()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        SimulateGroupStageInternal(fixture.CreateEngine());
        _notificationService.NotifyJournalists("Se simularon los resultados de los partidos pendientes.");
    }

    private void SimulateMatchInternal(int matchId, int seed, ISimulationEngine engine)
    {
        var match = _matchRepository.GetMatchById(matchId);
        SimulateWithEngine(match, seed, engine, resolveTie: true);
    }

    private void SimulateGroupStageInternal(ISimulationEngine engine)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        fixture.LoadMatches(_matchRepository.GetAllMatches());
        EnsureValidEngine(engine);

        foreach (var group in fixture.Groups)
            foreach (var match in group.GetPendingMatches())
                SimulateAndSetResult(match, _config.SimulationSeed, engine);
    }

    private void SimulateAndSetResult(Match match, int seed, ISimulationEngine engine)
    {
        if (match.HomeGoals.HasValue && match.AwayGoals.HasValue) return;
        SimulateWithEngine(match, seed, engine, resolveTie: false);
    }

    private void SimulateWithEngine(Match match, int seed, ISimulationEngine engine, bool resolveTie)
    {
        var (homeGoals, awayGoals) = RunEngine(match, seed, engine);

        match.ClearCardIncidents();
        AddSimulatedCardIncidents(match, seed);

        if (resolveTie && homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5) homeGoals++;
            else awayGoals++;
        }

        PersistMatchResult(match, homeGoals, awayGoals);
    }

    private (int homeGoals, int awayGoals) RunEngine(Match match, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);
        LastUsedEngineName = engine.Name;
        var matchSeed = seed + match.IdMatch;
        var result = engine.SimulateMatch(match.GetHomeRanking(), match.GetAwayRanking(), matchSeed);
        RegisterSimulation(seed, engine.Name);
        return result;
    }

    private void RegisterSimulation(int seed, string engineName)
    {
        _logService.Register(
            "system", "Simulation", "SimulationExecuted",
            $"Se ejecutó una simulación con semilla {seed} usando el motor {engineName}.");
    }

    private static void AddSimulatedCardIncidents(Match match, int seed)
    {
        var random = new Random(seed + match.IdMatch * 17);
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.HomeTeam, random.Next(0, MaxYellowCards + 1));
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.AwayTeam, random.Next(0, MaxYellowCards + 1));
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.HomeTeam, random.Next(0, MaxRedCards + 1));
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.AwayTeam, random.Next(0, MaxRedCards + 1));
    }

    private static void EnsureValidEngine(ISimulationEngine engine)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine), "El motor de simulación es obligatorio.");
    }

    private void PersistMatchResult(Match match, int homeGoals, int awayGoals)
    {
        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.UpdateMatch(match);
        UpdateRankingsAfterMatch(match);
    }

    private void UpdateRankingsAfterMatch(Match match)
    {
        if (!match.HomeGoals.HasValue || !match.AwayGoals.HasValue)
            throw new InvalidOperationException("El partido no tiene resultado.");

        var phase = match.GetTournamentPhase();
        int prevHome = match.GetHomeRanking();
        int prevAway = match.GetAwayRanking();

        match.RecalculateRankings(phase);

        _teamRepository.Update(match.HomeTeam);
        _teamRepository.Update(match.AwayTeam);

        _logService.Register(
            "system", "Ranking", "RankingUpdated",
            $"{match.GetHomeTeamName()}: {prevHome} -> {match.GetHomeRanking()}. " +
            $"{match.GetAwayTeamName()}: {prevAway} -> {match.GetAwayRanking()}.");
    }

    private void RevertRankingsAfterMatch(Match match)
    {
        var phase = match.GetTournamentPhase();
        match.RevertRankings(phase);
        _teamRepository.Update(match.HomeTeam);
        _teamRepository.Update(match.AwayTeam);
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
