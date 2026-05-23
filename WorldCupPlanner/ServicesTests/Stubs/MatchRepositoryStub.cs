using Domain.Entities;
using IRepository;

namespace ServicesTests.Stubs;

public class MatchRepositoryStub : IMatchRepository
{
    private List<Match> _matches = new();

    public int GetNextId()
    {
        throw new NotImplementedException();
    }

    List<Match> IMatchRepository.GetAllMatches()
    {
        return GetAllMatches();
    }

    Match IMatchRepository.GetMatchById(int matchId)
    {
        return GetMatchById(matchId);
    }

    public void AddMatch(Match match)
    {
        throw new NotImplementedException();
    }

    public List<Match> GetAllMatches() => _matches.ToList();
    public Match GetMatchById(int id) => _matches.First(m => m.IdMatch == id);
    public void ClearMatches() => _matches.Clear();
}