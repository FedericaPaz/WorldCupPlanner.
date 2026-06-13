using Domain.Entities;
using IRepository;
using IServices;

namespace Services;

public class RankingService : IRankingService
{
    private readonly ILogService _logService;
    private readonly ITeamRepository _teamRepository;

    public RankingService(ILogService logService, ITeamRepository teamRepository)
    {
        _logService = logService;
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
        var fase = match.GetTournamentPhase();

        int prevHome = homeTeam.CurrentRanking;
        int prevAway = awayTeam.CurrentRanking;

        homeTeam.RecalcularRanking(awayTeam.CurrentRanking, match.GetResultForHomeTeam(), fase);
        awayTeam.RecalcularRanking(homeTeam.CurrentRanking, match.GetResultForAwayTeam(), fase);

        _teamRepository.Update(homeTeam);
        _teamRepository.Update(awayTeam);

        _logService.Register(
            "system", "Ranking", "RankingUpdated",
            $"{homeTeam.Name}: {prevHome} -> {homeTeam.CurrentRanking}. " +
            $"{awayTeam.Name}: {prevAway} -> {awayTeam.CurrentRanking}.");
    }
}