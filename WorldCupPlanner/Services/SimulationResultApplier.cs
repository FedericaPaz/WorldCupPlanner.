using Domain.Entities;
using IRepository;
using IServices;

namespace Services;

public class SimulationResultApplier
{
    private readonly IMatchRepository _matchRepository;
    private readonly IRankingService _rankingService;

    public SimulationResultApplier(IMatchRepository matchRepository, IRankingService rankingService)
    {
        _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
        _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
    }

    public void ApplyAndPersistResult(Match match, int homeGoals, int awayGoals)
    {
        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.UpdateMatch(match);
        _rankingService.UpdateRankingsAfterMatch(match);
    }
}
