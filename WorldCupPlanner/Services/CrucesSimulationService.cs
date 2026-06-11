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

    public string LastUsedEngineName { get; private set; } = string.Empty;

    public CrucesSimulationService(ILogService logService, IMatchRepository matchRepository)
    {
        _logService = logService;
        _matchRepository = matchRepository;
    }

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        LastUsedEngineName = engine.Name;
//REVISAR
        var result = engine.SimulateMatch(match.HomeTeam.CurrentRanking,match.AwayTeam.CurrentRanking, seed);

        RegisterSimulation(seed, engine.Name);

        return result;
    }

    public void SimulateGroupStage(List<Group> groups, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        foreach (var group in groups)
        {
            foreach (var match in group.MatchesInGroup.Where(match => !match.HomeGoals.HasValue))
            {
                SimulateAndSetResult(match, seed, engine);
            }
        }
    }

    public void SimulateCruces(List<Cruce> cruces, int seed, ISimulationEngine engine)
    {
        EnsureValidEngine(engine);

        foreach (var cruce in cruces.Where(cruce => cruce.GetWinner() == null))
        {
            SimulateAndSetResult(cruce.Match, seed, engine);
        }
    }

    private void SimulateAndSetResult(Match match, int seed, ISimulationEngine engine)
    {
        var (homeGoals, awayGoals) = SimulateMatch(match, seed, engine);

        if (homeGoals == awayGoals)
        {
            var tiebreaker = new Random(seed + match.IdMatch * 31 + 1);

            if (tiebreaker.NextDouble() > 0.5)
            {
                homeGoals++;
            }
            else
            {
                awayGoals++;
            }
        }

        match.ClearCardIncidents();
        AddSimulatedCardIncidents(match, seed);
        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.UpdateMatch(match);
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