using Domain.Entities;
using IServices;

namespace Services;

public class RankingService : IRankingService
{
    private const int K = 30;
    private const double GroupPhaseMultiplier = 1.0;
    private const double KnockoutPhaseMultiplier = 1.5;
    private const char KnockoutGroup = 'N';

    private readonly ILogService _logService;

    public RankingService(ILogService logService)
    {
        _logService = logService;
    }

    public void UpdateRankingsAfterMatch(Match match)
    {
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        if (!match.HomeGoals.HasValue || !match.AwayGoals.HasValue)
        {
            throw new InvalidOperationException("El partido no tiene resultado.");
        }

        var homeTeam = (Team)match.HomeTeam;
        var awayTeam = (Team)match.AwayTeam;

        int previousHomeRanking = homeTeam.CurrentRanking;
        int previousAwayRanking = awayTeam.CurrentRanking;

        int newHomeRanking = CalculateNewRanking(
            homeTeam.CurrentRanking,
            awayTeam.CurrentRanking,
            GetResultForHomeTeam(match),
            GetPhaseMultiplier(match));

        int newAwayRanking = CalculateNewRanking(
            awayTeam.CurrentRanking,
            homeTeam.CurrentRanking,
            GetResultForAwayTeam(match),
            GetPhaseMultiplier(match));

        homeTeam.UpdateCurrentRanking(newHomeRanking);
        awayTeam.UpdateCurrentRanking(newAwayRanking);

        RegisterRankingUpdate(
            homeTeam,
            previousHomeRanking,
            newHomeRanking,
            awayTeam,
            previousAwayRanking,
            newAwayRanking);
    }

    private void RegisterRankingUpdate(
        Team homeTeam,
        int previousHomeRanking,
        int newHomeRanking,
        Team awayTeam,
        int previousAwayRanking,
        int newAwayRanking)
    {
        _logService.Register(
            "system",
            "Ranking",
            "RankingUpdated",
            $"{homeTeam.Name}: {previousHomeRanking} -> {newHomeRanking}. {awayTeam.Name}: {previousAwayRanking} -> {newAwayRanking}.");
    }

    private static int CalculateNewRanking(
        int currentRanking,
        int opponentRanking,
        double result,
        double phaseMultiplier)
    {
        double probability = 1 / (1 + Math.Pow(10, (opponentRanking - currentRanking) / 1000.0));
        double updatedRanking = currentRanking + (K * (result - probability)) * phaseMultiplier;

        return ClampRanking(RoundHalfUp(updatedRanking));
    }

    private static double GetResultForHomeTeam(Match match)
    {
        if (match.HomeGoals > match.AwayGoals)
        {
            return 1;
        }

        if (match.HomeGoals == match.AwayGoals)
        {
            return 0.5;
        }

        return 0;
    }

    private static double GetResultForAwayTeam(Match match)
    {
        if (match.AwayGoals > match.HomeGoals)
        {
            return 1;
        }

        if (match.HomeGoals == match.AwayGoals)
        {
            return 0.5;
        }

        return 0;
    }

    private static double GetPhaseMultiplier(Match match)
    {
        return match.Group == KnockoutGroup
            ? KnockoutPhaseMultiplier
            : GroupPhaseMultiplier;
    }

    private static int RoundHalfUp(double value)
    {
        return (int)Math.Floor(value + 0.5);
    }

    private static int ClampRanking(int ranking)
    {
        return Math.Max(Team.MinRanking, Math.Min(Team.MaxRanking, ranking));
    }
}