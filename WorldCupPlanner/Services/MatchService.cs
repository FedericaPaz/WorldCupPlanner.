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

    public MatchResponse Create(MatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        var stadium = _stadiumRepository.GetByName(request.stadiumName);
        var homeTeam = _teamRepository.GetByName(request.homeTeamName);
        var awayTeam = _teamRepository.GetByName(request.awayTeamName);
        var match = new Match(request.dateMatch, stadium, homeTeam, awayTeam, request.group[0], _clock);
        _matchRepository.AddMatch(match);
        return match.ToResponse();
    }
}