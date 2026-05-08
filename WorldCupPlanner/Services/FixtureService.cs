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
        throw new NotImplementedException();
    }
    
    public bool HasFixture()
    {
        throw new NotImplementedException();
    }
    
    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
    {
        throw new NotImplementedException();
    }
}