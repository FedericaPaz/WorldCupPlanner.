using Domain;
using Domain.Entities;
using Domain.Interfaces;
using DTOs.Fixture;
using IRepository;
using IServices.FixtureInterfaces;
using Services.Mappers;

namespace Services;

public class FixtureService : IFixtureEditService, IFixtureQueryService
{
    private readonly IFixtureRepository _fixtureRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ICrucesRepository _crucesRepository;
    private readonly TournamentConfig _config;
    private readonly IDateTimeMatch _clock;

    private const int TeamsPerGroup = 4;
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;
    private const int GroupsPerBatch = 6;

    private int _nextMatchId = 1;

    public FixtureService(
        IFixtureRepository fixtureRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IMatchRepository matchRepository,
        ICrucesRepository crucesRepository,
        TournamentConfig config,
        IDateTimeMatch clock)
    {
        _fixtureRepository = fixtureRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _matchRepository = matchRepository;
        _crucesRepository = crucesRepository;
        _config = config;
        _clock = clock;
    }

    public FixtureResponse GetFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return new FixtureResponse();

        var fixture = _fixtureRepository.GetFixture();
        fixture.LoadMatches(_matchRepository.GetAllMatches());
        return fixture.ToEntity();
    }

    public Fixture GetFixtureEntity()
    {
        if (!_fixtureRepository.ContainsFixture())
            throw new InvalidOperationException("No hay fixture generado.");

        var fixture = _fixtureRepository.GetFixture();
        fixture.LoadMatches(_matchRepository.GetAllMatches());
        return fixture;
    }

    public bool HasFixture()
    {
        try
        {
            ValidateExistingFixture();
            return _fixtureRepository.ContainsFixture();
        }
        catch
        {
            return false;
        }
    }

    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
    {
        var fixture = GetFixtureEntity();
        var group = fixture.Groups.FirstOrDefault(g => g.GroupLetter == groupLetter);
        if (group == null)
            throw new ArgumentException($"Grupo {groupLetter} no encontrado");

        return group.GetStandings()
            .Select(s => StandingMapper.ToResponse(s))
            .ToList();
    }

    public void GenerateFixture(string simulationEngineName = "Probabilistico")
    {
        _crucesRepository.RemoveCruces();
        _matchRepository.ClearMatches();

        if (_fixtureRepository.ContainsFixture())
            _fixtureRepository.RemoveFixture(_fixtureRepository.GetFixture());

        var teams = _teamRepository.GetAll();
        foreach (var team in teams)
        {
            team.ResetRankingToFifa();
            _teamRepository.Update(team);
        }

        _nextMatchId = _matchRepository.GetNextId();
        var fixture = GenerateGroupsStageFixture();

        foreach (var grupo in fixture.Groups)
        foreach (var partido in grupo.MatchesInGroup)
            _matchRepository.AddMatch(partido);

        fixture.SetSimulationEngine(simulationEngineName);
        _fixtureRepository.AddFixture(fixture);
    }

    private Fixture GenerateGroupsStageFixture()
    {
        var teams = _teamRepository.GetAll();
        var stadiums = _stadiumRepository.GetAllStadiums();
        
        var builder = new FixtureBuilder(_config, _clock);
        return builder.BuildGroupsStageFixture(teams, stadiums, ref _nextMatchId);
    }

    private void ValidateExistingFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return;

        var fixture = GetFixtureEntity();

        if (!fixture.IsStillValid(
                _teamRepository.GetAll().Cast<ITeam>().ToList(),
                _stadiumRepository.GetAllStadiums().Cast<IStadium>().ToList()))
        {
            _fixtureRepository.RemoveFixture(fixture);
        }
    }

    private static string NormalizeName(string name)
    {
        var lower = name.ToLowerInvariant();
        var normalized = string.Concat(lower.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));
        return string.Join(" ", normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    public void DeleteFixture()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        _crucesRepository.RemoveCruces();
        _matchRepository.ClearMatches();
        _fixtureRepository.RemoveFixture(fixture);

        var teams = _teamRepository.GetAll();
        foreach (var team in teams)
        {
            team.ResetRankingToFifa();
            _teamRepository.Update(team);
        }
    }
}