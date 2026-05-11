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
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
        List<Group> grupos = fixture.GetGroups.ToList();
        List<Stadium> estadios = _stadiumRepository.GetAllStadiums();
        List<Cruce> cruces = _generator.GenerateRoundOf32(grupos, estadios, 123);
        _repository.RemoveCruces();
        _repository.AddCruces(cruces);
    }

    public void GenerateNextRound()
    {
        List<Cruce> actuales = _repository.GetAllCruces();
        if (actuales == null || actuales.Count == 0)
            throw new ArgumentNullException(nameof(actuales));
        TournamentPhase ultimaFase = actuales.Max(x => x.Phase);
        if(ultimaFase == TournamentPhase.Final)
            throw new InvalidOperationException("No se pueden generar cruces para la siguiente fase, ya se encuentra en la fase final.");
        List<Cruce> ultimaRonda = actuales.Where(x => x.Phase == ultimaFase).ToList();
        List<Cruce> nuevos = _generator.GenerateNextRound(ultimaRonda, _stadiumRepository.GetAllStadiums(), 123);
        _repository.AddCruces(nuevos);
    }

    public List<CruceResponse> GetAllCruces()
    {
        throw new NotImplementedException();
    }
}