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
    
    public FixtureService(IFixtureRepository fixtureRepository, FixtureGenerator fixtureGenerator)
    {
        _fixtureRepository = fixtureRepository;
        _fixtureGenerator = fixtureGenerator;
    }
    
    public FixtureResponse GetFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return new FixtureResponse();
        
        return _fixtureRepository.GetFixture().ToEntity();
    }
    
    public bool HasFixture()
    {
        return _fixtureRepository.ContainsFixture();
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
    }
}