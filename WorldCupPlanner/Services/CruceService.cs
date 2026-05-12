using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class CruceService : ICruceService
{
    private readonly ICrucesRepository _repository;
    private ICrucesRepository _crucesSimuladosRepository;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly ICrucesGeneratorService _generator;
    private readonly ISimulationService _simulationService;
    private readonly TournamentConfig _config;

    public void SetRepository(ICrucesRepository repository)
    {
        _crucesSimuladosRepository = repository;
    }

    public CruceService(ICrucesRepository repository, IFixtureRepository fixtureRepository, IStadiumRepository stadiumRepository, ICrucesGeneratorService generator, ISimulationService simulationService, TournamentConfig config)
    {
        _repository = repository;
        _crucesSimuladosRepository = repository;
        _fixtureRepository = fixtureRepository;
        _stadiumRepository = stadiumRepository;
        _generator = generator;
        _simulationService = simulationService;
        _config = config;
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
        return _repository.GetAllCruces().Select(CruceMapper.ToResponse).ToList();;
    }

    public void SimulateCruces()
    {
        var cruces = _repository.GetAllCruces();
        _simulationService.SimulateCruces(cruces, _config.SemillaSimulation);
    }

    public void SimulateAndAdvanceToFinal()
    {
        var todos = _crucesSimuladosRepository.GetAllCruces();
        if (todos == null || todos.Count == 0)
            throw new InvalidOperationException("No hay cruces generados.");
        
        _crucesSimuladosRepository.RemoveCruces();
        var fixture = _fixtureRepository.GetFixture();
        var grupos = fixture.GetGroups.ToList();
        var estadios = _stadiumRepository.GetAllStadiums();
        var rondaActual = _generator.GenerateRoundOf32(grupos, estadios, _config.SemillaCrucesFase);

        _simulationService.SimulateCruces(rondaActual, _config.SemillaSimulation);
        _crucesSimuladosRepository.AddCruces(rondaActual);
        
        while (rondaActual.Max(x => x.Phase) != TournamentPhase.Final)
        {
            var nuevos = _generator.GenerateNextRound(rondaActual, estadios, _config.SemillaCrucesFase);
            _simulationService.SimulateCruces(nuevos, _config.SemillaSimulation);
            _crucesSimuladosRepository.AddCruces(nuevos);
            rondaActual = nuevos;
        }
    }
}