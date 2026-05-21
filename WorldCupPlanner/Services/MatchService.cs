using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Match;
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
    private readonly ISimulationService _simulationService;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly TournamentConfig _config;

    public MatchService(
        IMatchRepository matchRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IDateTimeMatch clock,
        ISimulationService simulationService,
        IFixtureRepository fixtureRepository,
        TournamentConfig config)
    {
        _matchRepository = matchRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _clock = clock;
        _simulationService = simulationService;
        _fixtureRepository = fixtureRepository;
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
        EnsureValidResultRequest(request);

        var match = _matchRepository.GetMatchById(matchId);

        AddCardIncidents(match, request);

        match.SetMatchResult(request.homeGoals, request.awayGoals);
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

    public void SimulateMatch(int matchId, int seed)
    {
        var match = _matchRepository.GetMatchById(matchId);
        var (home, away) = _simulationService.SimulateMatch(match, seed);

        match.SetMatchResult(home, away);
    }

    public void SimulateGroupStage()
    {
        var fixture = _fixtureRepository.GetFixture();

        if (fixture == null)
            throw new InvalidOperationException("No hay fixture generado.");

        var groups = fixture.Groups.ToList();

        _simulationService.SimulateGroupStage(groups, _config.SemillaSimulation);
    }

    private static void EnsureValidRequest(MatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.group))
            throw new ArgumentException("El grupo es obligatorio.");
    }

    private static void EnsureValidResultRequest(MatchResultRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
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

    private static void AddCardIncidents(Match match, MatchResultRequest request)
    {
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.HomeTeam, request.homeYellowCards);
        AddIncidentIfNeeded(match, IncidentType.YellowCard, match.AwayTeam, request.awayYellowCards);
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.HomeTeam, request.homeRedCards);
        AddIncidentIfNeeded(match, IncidentType.RedCard, match.AwayTeam, request.awayRedCards);
    }

    private static void AddIncidentIfNeeded(
        Match match,
        IncidentType type,
        ITeam team,
        int quantity)
    {
        if (quantity > 0)
        {
            match.AddIncident(new MatchIncident(type, (Team)team, quantity));
        }
    }
}