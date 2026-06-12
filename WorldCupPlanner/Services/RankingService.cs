using Domain.Entities;
using Domain.Calculators;
using Domain.Enums;
using IRepository;
using IServices;

namespace Services;

public class RankingService : IRankingService
{
    private readonly RankingCalculator _rankingCalculator;
    private readonly ILogService _logService;
    private readonly ITeamRepository _teamRepository;

    public RankingService(ILogService logService, RankingCalculator rankingCalculator, ITeamRepository teamRepository)
    {
        _logService = logService;
        _rankingCalculator = rankingCalculator;
        _teamRepository = teamRepository;
    }

    public void UpdateRankingsAfterMatch(Match match)
    {
        Console.WriteLine("entre a actualizar rankig");
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        if (!match.HomeGoals.HasValue || !match.AwayGoals.HasValue)
            throw new InvalidOperationException("El partido no tiene resultado.");

        var homeTeam = match.HomeTeam;
        var awayTeam = match.AwayTeam;

        int previousHomeRanking = homeTeam.CurrentRanking;
        int previousAwayRanking = awayTeam.CurrentRanking;
        Console.WriteLine($"home team: {homeTeam.Name} ranking: {homeTeam.CurrentRanking}");
        Console.WriteLine($"away team: {awayTeam.Name} ranking: {awayTeam.CurrentRanking}");
        
        TournamentPhase phase = GetPhase(match);

        int newHomeRanking = _rankingCalculator.CalculateNewRanking(
            homeTeam.CurrentRanking,
            awayTeam.CurrentRanking,
            GetResultForHomeTeam(match),
            phase);

        int newAwayRanking = _rankingCalculator.CalculateNewRanking(
            awayTeam.CurrentRanking,
            homeTeam.CurrentRanking,
            GetResultForAwayTeam(match),
            phase);

        homeTeam.UpdateCurrentRanking(newHomeRanking);
        awayTeam.UpdateCurrentRanking(newAwayRanking);
        
        Console.WriteLine($"Actualizando ranking {homeTeam.Name}: {previousHomeRanking} -> {newHomeRanking}");
        Console.WriteLine($"Actualizando ranking {awayTeam.Name}: {previousAwayRanking} -> {newAwayRanking}");
        
        _teamRepository.Update(homeTeam);
        _teamRepository.Update(awayTeam);

        RegisterRankingUpdate(
            homeTeam, previousHomeRanking, newHomeRanking,
            awayTeam, previousAwayRanking, newAwayRanking);
        
        Console.WriteLine($"new home team: {homeTeam.Name} ranking: {homeTeam.CurrentRanking}");
        Console.WriteLine($"new away team: {awayTeam.Name} ranking: {awayTeam.CurrentRanking}");
    }
    
    private static TournamentPhase GetPhase(Match match)
    {
        return match.Group == 'N'
            ? TournamentPhase.RoundOf32
            : TournamentPhase.GroupStage;
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
    
    private static double GetResultForHomeTeam(Match match)
    {
        if (match.HomeGoals > match.AwayGoals) return 1;
        if (match.HomeGoals == match.AwayGoals) return 0.5;
        return 0;
    }

    private static double GetResultForAwayTeam(Match match)
    {
        if (match.AwayGoals > match.HomeGoals) return 1;
        if (match.HomeGoals == match.AwayGoals) return 0.5;
        return 0;
    }
}