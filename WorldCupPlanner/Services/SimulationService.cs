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

    public string LastUsedEngineName { get; private set; } = string.Empty;

    public SimulationService(
        ILogService logService,
        IMatchRepository matchRepository,
        IRankingService rankingService,
        IFixtureRepository fixtureRepository,
        TournamentConfig config)
    {
        _logService = logService;
        _matchRepository = matchRepository;
        _rankingService = rankingService;
        _fixtureRepository = fixtureRepository;
        _config = config;
    }

    public void SimulateMatch(int matchId, int seed)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        SimulateMatch(matchId, seed, fixture.CreateEngine());
    }

    public void SimulateMatch(int matchId, int seed, ISimulationEngine engine)
    {
        var match = _matchRepository.GetMatchById(matchId);
        var (home, away) = SimulateMatch(match, seed, engine);
        var random = new Random(seed);

        var homeGoals = home;
        var awayGoals = away;

        // Generate card incidents (clears previous card incidents internally)
        CardIncidentGenerator.Generate(match, seed);

        if (homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5) homeGoals++;
            else awayGoals++;
        }

        // Apply result and persist using dedicated applier
        var applier = new SimulationResultApplier(_matchRepository, _rankingService);
        applier.ApplyAndPersistResult(match, homeGoals, awayGoals);
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

        SimulateGroupStage(fixture.CreateEngine());
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

        // Generate card incidents and apply result via applier
        CardIncidentGenerator.Generate(match, seed);
        var applier = new SimulationResultApplier(_matchRepository, _rankingService);
        applier.ApplyAndPersistResult(match, homeGoals, awayGoals);
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
        CardIncidentGenerator.Generate(match, seed);
    }

    private static void EnsureValidEngine(ISimulationEngine engine)
    {
        if (engine == null)
            throw new ArgumentNullException(nameof(engine), "El motor de simulación es obligatorio.");
    }
}