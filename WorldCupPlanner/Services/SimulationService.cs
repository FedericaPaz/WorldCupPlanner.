using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using IRepository;
using IServices;

namespace Services;

public class SimulationService : ISimulationService
{
    private readonly ILogService _logService;
    private readonly IMatchRepository _matchRepository;
    private readonly IRankingService _rankingService;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly TournamentConfig _config;
    private readonly INotificationService _notificationService;
    private readonly Services.Simulation.ISimulationEngineFactory _engineFactory;

    public string LastUsedEngineName { get; private set; } = string.Empty;

    public SimulationService(
        ILogService logService,
        IMatchRepository matchRepository,
        IRankingService rankingService,
        IFixtureRepository fixtureRepository,
        TournamentConfig config,
        Services.Simulation.ISimulationEngineFactory engineFactory,
        INotificationService notificationService)
    {
        _logService = logService;
        _matchRepository = matchRepository;
        _rankingService = rankingService;
        _fixtureRepository = fixtureRepository;
        _config = config;
        _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void SimulateMatch(int matchId, int seed)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        var engine = _engineFactory.Create(fixture.SimulationEngineName);

        var match = _matchRepository.GetMatchById(matchId);
        LastUsedEngineName = engine.Name;

        var coordinator = new Domain.Simulation.SimulationCoordinator();
        coordinator.SimulateAndApply(match, seed, engine, resolverEmpates: false);

        RegisterSimulation(seed, engine.Name);

        _matchRepository.UpdateMatch(match);
        _rankingService.UpdateRankingsAfterMatch(match);

        _notificationService.NotifyJournalists("Se simuló el resultado de un partido.");
    }

    public void SimulateGroupStage()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        var engine = _engineFactory.Create(fixture.SimulationEngineName);
        SimulateGroupStage(engine);

        _notificationService.NotifyJournalists("Se simularon los resultados de los partidos pendientes.");
    }

    private void SimulateGroupStage(ISimulationEngine engine)
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        fixture.CargarPartidos(_matchRepository.GetAllMatches());
        SimulateGroupStage(fixture.Groups.ToList(), _config.SemillaSimulation, engine);
    }

    private void SimulateGroupStage(List<Group> groups, int seed, ISimulationEngine engine)
    {
        var coordinator = new Domain.Simulation.SimulationCoordinator();

        foreach (var group in groups)
            foreach (var match in group.GetPendingMatches())
                SimulateAndSetResult(match, seed, engine, false, coordinator);
    }

    private void SimulateAndSetResult(Match match, int seed, ISimulationEngine engine, bool resolverEmpates, Domain.Simulation.SimulationCoordinator coordinator)
    {
        if (match.HomeGoals.HasValue && match.AwayGoals.HasValue) return;

        coordinator.SimulateAndApply(match, seed, engine, resolverEmpates);
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
}
