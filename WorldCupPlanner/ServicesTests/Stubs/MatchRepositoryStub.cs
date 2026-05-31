using Domain.Entities;
using IRepository;

namespace ServicesTests.Stubs;

public class MatchRepositoryStub : IMatchRepository
{
    private readonly List<Match> _matches = new();

    public void AddMatch(Match match)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        _matches.Add(match);
    }

    public int GetNextId()
    {
        return _matches.Count == 0
            ? 1
            : _matches.Max(m => m.IdMatch) + 1;
    }

    public List<Match> GetAllMatches()
    {
        return _matches.ToList();
    }

    public Match GetMatchById(int matchId)
    {
        if (matchId <= 0)
            throw new ArgumentException("El id debe ser mayor a 0.");

        var match = _matches.FirstOrDefault(m => m.IdMatch == matchId);

        if (match == null)
            throw new KeyNotFoundException("No existe un partido con ese id.");

        return match;
    }

    public void ClearMatches()
    {
        _matches.Clear();
    }
}