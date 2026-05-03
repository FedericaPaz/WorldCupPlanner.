using DTOs;
using Domain;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IDateTimeMatch _clock;

    public MatchService(IMatchRepository matchRepository, ITeamRepository teamRepository, IStadiumRepository stadiumRepository, IDateTimeMatch clock)
    {
        _matchRepository = matchRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _clock = clock;
    }

    public MatchResponse CreateMatch(MatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.group))
            throw new ArgumentException("El grupo es obligatorio.");
        var stadium = _stadiumRepository.GetByName(request.stadiumName);
        var homeTeam = _teamRepository.GetByName(request.homeTeamName);
        var awayTeam = _teamRepository.GetByName(request.awayTeamName);
        var match = new Match(request.dateMatch, stadium, homeTeam, awayTeam, request.group[0], _clock);
        _matchRepository.AddMatch(match);
        return match.ToResponse();
    }

    public void SetMatchResult(int matchId, int homeGoals, int awayGoals)
    {
        var match = _matchRepository.GetMatchById(matchId);
        match.SetMatchResult(homeGoals, awayGoals);
    }
    
    public MatchResponse GetMatchById(int matchId)
    {
        var match = _matchRepository.GetMatchById(matchId);
        return match.ToResponse();
        
    }
    
    public List<MatchResponse> GetAllMatches()
    {
        return _matchRepository
            .GetAllMatches()
            .Select(match => match.ToResponse())
            .ToList();
    }

    public void UpdateMatchDate(int matchId, DateTime date)
    {
        var match = _matchRepository.GetMatchById(matchId);
        match.UpdateDate(date);
    }

    public void UpdateMatchStadium(int matchId, string stadiumName)
    {
        var match = _matchRepository.GetMatchById(matchId);
        var stadium = _stadiumRepository.GetByName(stadiumName);
        match.UpdateStadium(stadium);
    }
}