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

    public MatchService(
        IMatchRepository matchRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IDateTimeMatch clock)
    {
        _matchRepository = matchRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _clock = clock;
    }

    public MatchResponse CreateMatch(MatchRequest request)
    {
        ValidateRequest(request);

        var stadium = GetStadiumOrThrow(request.stadiumName);
        var homeTeam = GetTeamOrThrow(request.homeTeamName, "local");
        var awayTeam = GetTeamOrThrow(request.awayTeamName, "visitante");

        var match = CreateMatchEntity(request, stadium, homeTeam, awayTeam);

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
        var stadium = GetStadiumOrThrow(stadiumName);

        match.UpdateStadium(stadium);
    }

    private static void ValidateRequest(MatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.group))
            throw new ArgumentException("El grupo es obligatorio.");
    }

    private IStadium GetStadiumOrThrow(string stadiumName)
    {
        var stadium = _stadiumRepository.GetStadium(stadiumName);

        if (stadium == null)
            throw new KeyNotFoundException("No existe un estadio con ese nombre.");

        return stadium;
    }

    private ITeam GetTeamOrThrow(string teamName, string teamType)
    {
        var team = _teamRepository.Get(teamName);

        if (team == null)
            throw new KeyNotFoundException($"No existe el equipo {teamType}.");

        return team;
    }

    private Match CreateMatchEntity(
        MatchRequest request,
        IStadium stadium,
        ITeam homeTeam,
        ITeam awayTeam)
    {
        return new Match(
            request.dateMatch,
            stadium,
            homeTeam,
            awayTeam,
            request.group[0],
            _clock);
    }
}