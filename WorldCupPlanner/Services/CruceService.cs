using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class CruceService : ICruceService
{
    private readonly ICrucesRepository _repository;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly ICruceGeneratorService _generator;

    public CruceService(ICrucesRepository repository, IFixtureRepository fixtureRepository, IStadiumRepository stadiumRepository, ICruceGeneratorService generator)
    {
        _repository = repository;
        _fixtureRepository = fixtureRepository;
        _stadiumRepository = stadiumRepository;
        _generator = generator;
    }

    public void GenerateRoundOf32()
    {
        Fixture fixture = _fixtureRepository.GetFixture();
        List<Group> grupos = fixture.GetGroups.ToList();
        List<Stadium> estadios = _stadiumRepository.GetAllStadiums();
        List<Cruce> cruces = _generator.GenerateRoundOf32(grupos, estadios, 123);
        _repository.RemoveCruces();
        _repository.AddCruces(cruces);
    }

    public void GenerateNextRound()
    {
        throw new NotImplementedException();
    }

    public List<CruceResponse> GetAllCruces()
    {
        throw new NotImplementedException();
    }
}