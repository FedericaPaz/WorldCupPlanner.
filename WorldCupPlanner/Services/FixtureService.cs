using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class FixtureService : IFixtureService
{
    private readonly IFixtureRepository _fixtureRepository;
    
    public FixtureService(IFixtureRepository fixtureRepository)
    {
        _fixtureRepository = fixtureRepository;
    }
    
    public FixtureResponse GetFixture()
    {
        try
        {
            var fixture = _fixtureRepository.GetFixture();
            if (_fixtureRepository.ContainsFixture(fixture))
            {
                return fixture.ToEntity();
            }
            return new FixtureResponse();
        }
        catch (InvalidOperationException)
        {
            return new FixtureResponse();
        }
    }
    
    public bool HasFixture()
    {
        try
        {
            var fixture = _fixtureRepository.GetFixture();
            return _fixtureRepository.ContainsFixture(fixture);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
    
    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
    {
        throw new NotImplementedException();
    }
}