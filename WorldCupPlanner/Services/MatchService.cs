using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Match;
using IRepository;
using IServices.MatchInterfaces;
using IServices;
using Services.Mappers;

namespace Services;

public class MatchService : IMatchQueryService, IMatchEditService, IMatchCreateService
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IDateTimeMatch _clock;
    private readonly INotificationService _notificationService;
    private readonly IRankingService _rankingService;
    private readonly TournamentConfig _config;            

    public MatchService(
        IMatchRepository matchRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IDateTimeMatch clock,
        INotificationService notificationService,
        IRankingService rankingService,
        TournamentConfig config)                        
    {
        _matchRepository = matchRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _clock = clock;
        _notificationService = notificationService;
        _rankingService = rankingService;
        _config = config;
    }

    public MatchResponse CreateMatch(MatchRequest request)
    {
        EnsureValidRequest(request);

        var stadium = GetStadiumOrThrow(request.stadiumName);
        var homeTeam = GetTeamOrThrow(request.homeTeamName, "local");
        var awayTeam = GetTeamOrThrow(request.awayTeamName, "visitante");

        var match = CreateMatchEntity(request, stadium, homeTeam, awayTeam);
        _matchRepository.AddMatch(match);

        return match.ToResponse();
    }

    public void SetMatchResult(int matchId, MatchResultRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var match = _matchRepository.GetMatchById(matchId);
        
        bool yaTeníaResultado = match.HomeGoals.HasValue && match.AwayGoals.HasValue;
        if (yaTeníaResultado)
            _rankingService.RevertRankingsAfterMatch(match);
        
        match.ClearAllIncidents();

        var incidents = MatchResultMapper.ToIncidents(request, match);
        foreach (var inc in incidents)
            match.AddIncident(inc);
        match.UpdateMatchResult(request.homeGoals, request.awayGoals);

        _matchRepository.UpdateMatch(match);
        _rankingService.UpdateRankingsAfterMatch(match);

        _notificationService.NotifyJournalists(
            $"Se cargó el resultado de {match.HomeTeam.Name} vs {match.AwayTeam.Name}: {request.homeGoals}-{request.awayGoals}");
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

    public bool MatchesHaveBeenPlayed()
    {
        return _matchRepository.GetAllMatches()
            .Any(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue);
    }

    private static void EnsureValidRequest(MatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.group))
            throw new ArgumentException("El grupo es obligatorio.");
    }

    private Stadium GetStadiumOrThrow(string stadiumName)
    {
        var stadium = _stadiumRepository.GetStadium(stadiumName);
        if (stadium == null)
            throw new KeyNotFoundException("No existe un estadio con ese nombre.");
        return stadium;
    }

    private Team GetTeamOrThrow(string teamName, string teamType)
    {
        var team = _teamRepository.Get(teamName);
        if (team == null)
            throw new KeyNotFoundException($"No existe el equipo {teamType}.");
        return team;
    }

    private Match CreateMatchEntity(MatchRequest request, Stadium stadium, Team homeTeam, Team awayTeam)
    {
        int id = _matchRepository.GetNextId();
        return new Match(id, request.dateMatch, stadium, homeTeam, awayTeam, request.group[0], _clock);
    }

}