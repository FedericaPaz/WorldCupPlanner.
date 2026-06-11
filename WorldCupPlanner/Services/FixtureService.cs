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
        {
            return new FixtureResponse();
        }
    
        var fixture = _fixtureRepository.GetFixture();
        
        ReconstructGroups(fixture);
        return fixture.ToEntity();
    }

    public Fixture GetFixtureEntity()
    {
        if (!_fixtureRepository.ContainsFixture())
        {
            throw new InvalidOperationException("No hay fixture generado.");
        }

        try
        {
            var fixture = _fixtureRepository.GetFixture();
            ReconstructGroups(fixture);
            return fixture;
        }
        catch
        {
            Console.WriteLine("Error al obtener el fixture en getFixtureEntity.");
            throw;
        }
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
            Console.WriteLine("fallo en hasFixture");
            return false;
        }
    }

    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
    {
        var fixture = GetFixtureEntity();
        var group = fixture.Groups.FirstOrDefault(g => g.GroupLetter == groupLetter);
        if (group == null)
            throw new ArgumentException($"Grupo {groupLetter} no encontrado");

        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        return standings.Select(s => StandingMapper.ToResponse(s)).ToList();
    }

    public void GenerateFixture(string simulationEngineName = "Probabilistico")
    {
        var fixture = _fixtureGenerator.GenerateGroupsStageFixture();

        _matchRepository.ClearMatches();
        _crucesRepository.RemoveCruces();

        foreach (var grupo in fixture.Groups)
        foreach (var partido in grupo.MatchesInGroup)
            _matchRepository.AddMatch(partido);

        fixture.SetSimulationEngine(simulationEngineName);
        _fixtureRepository.AddFixture(fixture);
    }
    
    private void ValidateExistingFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
        {
            return;
        }
        
        var fixture = GetFixtureEntity();

        if (!fixture.IsStillValid(
                _teamRepository.GetAll().Cast<ITeam>().ToList(),
                _stadiumRepository.GetAllStadiums().Cast<IStadium>().ToList()))
        {
            _fixtureRepository.RemoveFixture(fixture);
        }
    }

    private void ReconstructGroups(Fixture fixture)
    {
        var allMatches = _matchRepository.GetAllMatches();
        
        foreach (var group in fixture.Groups)
        {
            var groupMatches = allMatches
                .Where(m => m.Group == group.GroupLetter)
                .ToList();

            if (group.MatchesInGroup.Count == 6) break;
            
            foreach (var match in groupMatches)
                group.LoadMatchFromRepository(match);
        }
    }

    public void DeleteFixture(Fixture fixture)
    {
        _crucesRepository.RemoveCruces();
        _matchRepository.ClearMatches();
        _fixtureRepository.RemoveFixture(fixture);
    }
}