using Domain.Entities;
using IServices;

namespace ServicesTests.Stubs;

public class RankingServiceStub : IRankingService
{
    public void UpdateRankingsAfterMatch(Match match)
    {
    }

    public void RevertRankingsAfterMatch(Match match)
    {
        throw new NotImplementedException();
    }
}