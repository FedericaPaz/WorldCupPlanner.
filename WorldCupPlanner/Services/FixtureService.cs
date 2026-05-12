using Domain;
using DTOs;
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
    
    public FixtureService(IFixtureRepository fixtureRepository, FixtureGenerator fixtureGenerator,  ITeamRepository teamRepository, IStadiumRepository stadiumRepository, IMatchRepository matchRepository, ICrucesRepository crucesRepository)
    {
        _fixtureRepository = fixtureRepository;
        _fixtureGenerator = fixtureGenerator;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _matchRepository = matchRepository;
        _crucesRepository = crucesRepository;
    }
    
    public FixtureResponse GetFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return new FixtureResponse();
        
        return _fixtureRepository.GetFixture().ToEntity();
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
        var group = fixture.GetGroups.FirstOrDefault(g => g.GroupLetter == groupLetter);
        if (group == null)
            throw new ArgumentException($"Grupo {groupLetter} no encontrado");
            
        var calculator = new StandingCalculator();
        var standings = calculator.CalculateStandings(group);
        return standings.Select(s => StandingMapper.ToResponse(s)).ToList();
    }
    
    public void GenerateFixture()
    {
        var config = new TournamentConfig();
        _fixtureGenerator.GenerateGroupsStageFixture(config);

        var fixture = _fixtureRepository.GetFixture();
        var partidos = new List<Match>();
        foreach (var grupo in fixture.GetGroups)
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
        var fixture = _fixtureRepository.GetFixture();
        if (!fixture.IsStillValid(_teamRepository.GetAll(), _stadiumRepository.GetAllStadiums()))
            _fixtureRepository.RemoveFixture(fixture);
    }
}