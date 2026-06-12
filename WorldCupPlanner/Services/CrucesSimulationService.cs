using IServices;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;
using IRepository;

namespace Services;

public class CrucesSimulationService : ISimulationService
{
    private const int MaxYellowCards = 4;
    private const int MaxRedCards = 1;

    private readonly ILogService _logService;
    private readonly IMatchRepository _matchRepository;
    private readonly IRankingService _rankingService;

    public string LastUsedEngineName { get; private set; } = string.Empty;

    public CrucesSimulationService(ILogService logService, IMatchRepository matchRepository, IRankingService rankingService)
    {
        _logService = logService;
        _matchRepository = matchRepository;
        _rankingService = rankingService;
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

    public void SimulateGroupStage(List<Group> groups, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        foreach (var group in groups)
        {
            foreach (var match in group.MatchesInGroup.Where(match => !match.HomeGoals.HasValue && !match.AwayGoals.HasValue))
            {
                SimulateAndSetResult(match, seed, engine, false);
            }
        }
    }

    public void SimulateCruces(List<Cruce> cruces, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        foreach (var cruce in cruces.Where(cruce => cruce.GetWinner() == null))
            SimulateAndSetResult(cruce.Match, seed, engine, true);
    }

    private void SimulateAndSetResult(Match match, int seed, ISimulationEngine engine, bool empates)
    {
        if (match.HomeGoals.HasValue && match.AwayGoals.HasValue) return;

        var (homeGoals, awayGoals) = SimulateMatch(match, seed, engine);

        if (empates && homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);
            if (tiebreaker.NextDouble() > 0.5)
                homeGoals++;
            else
                awayGoals++;
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

        AddIncidentIfNeeded(
            match,
            IncidentType.YellowCard,
            match.HomeTeam,
            random.Next(0, MaxYellowCards + 1));

        AddIncidentIfNeeded(
            match,
            IncidentType.YellowCard,
            match.AwayTeam,
            random.Next(0, MaxYellowCards + 1));

        AddIncidentIfNeeded(
            match,
            IncidentType.RedCard,
            match.HomeTeam,
            random.Next(0, MaxRedCards + 1));

        AddIncidentIfNeeded(
            match,
            IncidentType.RedCard,
            match.AwayTeam,
            random.Next(0, MaxRedCards + 1));
    }

    private static void AddIncidentIfNeeded(
        Match match,
        IncidentType type,
        ITeam team,
        int quantity)
    {
        if (quantity > 0)
        {
            match.AddIncident(new MatchIncident(type, (Team)team, quantity));
        }
    }

    private static void EnsureValidEngine(ISimulationEngine engine)
    {
        if (engine == null)
        {
            throw new ArgumentNullException(nameof(engine), "El motor de simulación es obligatorio.");
        }
    }
}