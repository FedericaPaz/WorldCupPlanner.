using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using IRepository;
using IServices;

namespace Services;

public class SimulationService : ISimulationService
{
    private const int MaxYellowCards = 4;
    private const int MaxRedCards = 1;

    private readonly ILogService _logService;
    private readonly IMatchRepository _matchRepository;
    private readonly IRankingService _rankingService;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly TournamentConfig _config;
    private readonly ISimulationEngineFactory _engineFactory;

    public string LastUsedEngineName { get; private set; } = string.Empty;

    public SimulationService(
        ILogService logService,
        IMatchRepository matchRepository,
        IRankingService rankingService,
        IFixtureRepository fixtureRepository,
        TournamentConfig config,
        ISimulationEngineFactory engineFactory)
    {
        _logService = logService;
        _matchRepository = matchRepository;
        _rankingService = rankingService;
        _fixtureRepository = fixtureRepository;
        _config = config;
        _engineFactory = engineFactory;
    }

    public void SimulateMatch(int matchId, int seed)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        var engine = _engineFactory.Create(fixture.SimulationEngineName);
        SimulateMatch(matchId, seed, engine);
    }

    public void SimulateMatch(int matchId, int seed, ISimulationEngine engine)
    {
        var match = _matchRepository.GetMatchById(matchId);
        var (home, away) = SimulateMatch(match, seed, engine);
        var random = new Random(seed);

        var homeGoals = home;
        var awayGoals = away;

        match.ClearCardIncidents();
        AddSimulatedCardIncidents(match, seed);

        if (homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5) homeGoals++;
            else awayGoals++;
        }

        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.UpdateMatch(match);
        _rankingService.UpdateRankingsAfterMatch(match);
    }

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        LastUsedEngineName = engine.Name;

        var matchSeed = seed + match.IdMatch;
        var result = engine.SimulateMatch(match.HomeTeam.CurrentRanking, match.AwayTeam.CurrentRanking, matchSeed);

        RegisterSimulation(seed, engine.Name);

        return result;
    }

    public void SimulateGroupStage()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        var engine = _engineFactory.Create(fixture.SimulationEngineName);
        SimulateGroupStage(engine);
    }

    public void SimulateGroupStage(ISimulationEngine engine)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        fixture.CargarPartidos(_matchRepository.GetAllMatches());
        SimulateGroupStage(fixture.Groups.ToList(), _config.SemillaSimulation, engine);
    }

    public void SimulateGroupStage(List<Group> groups, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        foreach (var group in groups)
            foreach (var match in group.GetPendingMatches())
                SimulateAndSetResult(match, seed, engine, false);
    }

    public void SimulateCruces(List<Cruce> cruces, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        foreach (var cruce in cruces.Where(c => c.GetWinner() == null))
            SimulateAndSetResult(cruce.Match, seed, engine, true);
    }

    private void SimulateAndSetResult(Match match, int seed, ISimulationEngine engine, bool resolverEmpates)
    {
        if (match.HomeGoals.HasValue && match.AwayGoals.HasValue) return;

        var (homeGoals, awayGoals) = SimulateMatch(match, seed, engine);

        if (resolverEmpates && homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5) homeGoals++;
            else awayGoals++;
        }

        match.ClearCardIncidents();
        AddSimulatedCardIncidents(match, seed);
        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.UpdateMatch(match);
        _rankingService.UpdateRankingsAfterMatch(match);
    }

    private void RegisterSimulation(int seed, string engineName)
    {
        _logService.Register(
            "system",
            "Simulation",
            "SimulationExecuted",
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

    private static void AddIncidentIfNeeded(Match match, IncidentType type, ITeam team, int quantity)
    {
        if (quantity > 0)
            match.AddIncident(new MatchIncident(type, (Team)team, quantity));
    }

    private static void EnsureValidEngine(ISimulationEngine engine)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine), "El motor de simulación es obligatorio.");
    }
}