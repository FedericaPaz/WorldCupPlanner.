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
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        if (!match.HomeGoals.HasValue || !match.AwayGoals.HasValue)
            throw new InvalidOperationException("El partido no tiene resultado.");

        var homeTeam = match.HomeTeam;
        var awayTeam = match.AwayTeam;

        int previousHomeRanking = homeTeam.CurrentRanking;
        int previousAwayRanking = awayTeam.CurrentRanking;
        
        var phase = match.GetTournamentPhase();

        int newHomeRanking = _rankingCalculator.CalculateNewRanking(
            homeTeam.CurrentRanking,
            awayTeam.CurrentRanking,
            match.GetResultForHomeTeam(),
            phase);

        int newAwayRanking = _rankingCalculator.CalculateNewRanking(
            awayTeam.CurrentRanking,
            homeTeam.CurrentRanking,
            match.GetResultForAwayTeam(),
            phase);

        homeTeam.UpdateCurrentRanking(newHomeRanking);
        awayTeam.UpdateCurrentRanking(newAwayRanking);
        
        _teamRepository.Update(homeTeam);
        _teamRepository.Update(awayTeam);

        RegisterRankingUpdate(
            homeTeam, previousHomeRanking, newHomeRanking,
            awayTeam, previousAwayRanking, newAwayRanking);
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
}