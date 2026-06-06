using Domain.Calculators;
using Domain.Entities;
using Domain.Interfaces;
using DTOs.Fixture;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class FixtureService : IFixtureService
{
    private readonly IFixtureRepository _fixtureRepository;
    private readonly FixtureGenerator _fixtureGenerator;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ICrucesRepository _crucesRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly TournamentConfig _config;

    public FixtureService(
        IFixtureRepository fixtureRepository,
        FixtureGenerator fixtureGenerator,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IMatchRepository matchRepository,
        ICrucesRepository crucesRepository,
        IGroupRepository groupRepository,
        TournamentConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _fixtureRepository = fixtureRepository;
        _fixtureGenerator = fixtureGenerator;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _matchRepository = matchRepository;
        _crucesRepository = crucesRepository;
        _groupRepository = groupRepository;
        _config = config;
    }

    public FixtureResponse GetFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return new FixtureResponse();

        var fixture = _fixtureRepository.GetFixture();
        var allMatches = _matchRepository.GetAllMatches();
        var groups = _groupRepository.GetAllGroups();
        var allTeams = _teamRepository.GetAll();

        foreach (var group in groups)
        {
            var groupMatches = allMatches
                .Where(m => m.Group == group.GroupLetter)
                .ToList();

            var teamsInGroup = groupMatches
                .SelectMany(m => new[] { m.HomeTeam.Name, m.AwayTeam.Name })
                .Distinct()
                .Select(name => allTeams.First(t => t.Name == name))
                .ToList();

            foreach (var team in teamsInGroup)
                group.AddTeamToGroup(team);

            foreach (var match in groupMatches)
                group.LoadMatchFromRepository(match);
        }

        fixture.SetGroups(groups);
        return fixture.ToEntity();
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
        var fixture = _fixtureRepository.GetFixture();
        var group = fixture.Groups.FirstOrDefault(g => g.GroupLetter == groupLetter);
        if (group == null)
            throw new ArgumentException($"Grupo {groupLetter} no encontrado");

        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        return standings.Select(s => StandingMapper.ToResponse(s)).ToList();
    }

    public void GenerateFixture()
    {
        _fixtureGenerator.GenerateGroupsStageFixture();

        var fixture = _fixtureRepository.GetFixture();
        var partidos = new List<Match>();
        foreach (var grupo in fixture.Groups)
        {
            foreach (var partido in grupo.MatchesInGroup)
                partidos.Add(partido);
        }

        _matchRepository.ClearMatches();
        _crucesRepository.RemoveCruces();
        foreach (var match in partidos)
            _matchRepository.AddMatch(match);
    }
    
    private void ValidateExistingFixture()
    {
        if (!_fixtureRepository.ContainsFixture()) return;
        var fixture = GetFixtureEntity();
        if (!fixture.IsStillValid(
                _teamRepository.GetAll().Cast<ITeam>().ToList(),
                _stadiumRepository.GetAllStadiums().Cast<IStadium>().ToList()))
            _fixtureRepository.RemoveFixture(fixture);
    }
    public Fixture GetFixtureEntity()
    {
        if (!_fixtureRepository.ContainsFixture())
            throw new InvalidOperationException("No hay fixture generado.");

        var fixture = _fixtureRepository.GetFixture();
        var allMatches = _matchRepository.GetAllMatches();
        var groups = _groupRepository.GetAllGroups();
        var allTeams = _teamRepository.GetAll();

        foreach (var group in groups)
        {
            var groupMatches = allMatches
                .Where(m => m.Group == group.GroupLetter)
                .ToList();

            var teamsInGroup = groupMatches
                .SelectMany(m => new[] { m.HomeTeam.Name, m.AwayTeam.Name })
                .Distinct()
                .Select(name => allTeams.First(t => t.Name == name))
                .ToList();

            foreach (var team in teamsInGroup)
                group.AddTeamToGroup(team);

            foreach (var match in groupMatches)
                group.LoadMatchFromRepository(match);
        }

        fixture.SetGroups(groups);
        return fixture;
    }
}