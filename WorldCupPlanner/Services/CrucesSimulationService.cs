using IServices;
using Domain.Entities;

namespace Services;

public class CrucesSimulationService : ISimulationService
{
    private const int MaxGoals = 7;
    private const string SimulationEngineName = "CrucesSimulationService";

    private readonly ILogService _logService;

    public CrucesSimulationService(ILogService logService)
    {
        _logService = logService;
    }

    public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
    {
        var home = (Team)match.HomeTeam;
        var away = (Team)match.AwayTeam;
        var random = new Random(seed + match.IdMatch);

        double homeStrength = (double)home.FifaRanking / (home.FifaRanking + away.FifaRanking);

        int homeGoals = (int)Math.Round(homeStrength * random.Next(0, MaxGoals + 1));
        int awayGoals = (int)Math.Round((1 - homeStrength) * random.Next(0, MaxGoals + 1));

        RegisterSimulation(seed);

        return (homeGoals, awayGoals);
    }

    public void SimulateGroupStage(List<Group> groups, int seed)
    {
        foreach (var group in groups)
        {
            foreach (var match in group.MatchesInGroup.Where(match => !match.HomeGoals.HasValue))
            {
                SimulateAndSetResult(match, seed);
            }
        }
    }

    public void SimulateCruces(List<Cruce> cruces, int seed)
    {
        foreach (var cruce in cruces.Where(cruce => cruce.GetWinner() == null))
        {
            SimulateAndSetResult(cruce.Match, seed);
        }
    }

    private void SimulateAndSetResult(Match match, int seed)
    {
        var (homeGoals, awayGoals) = SimulateMatch(match, seed);

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

        match.SimulateResult(homeGoals, awayGoals);
    }

    private void RegisterSimulation(int seed)
    {
        _logService.Register("system", "Simulation", "SimulationExecuted", $"Se ejecutó una simulación con semilla {seed} usando el motor {SimulationEngineName}.");
    }
}