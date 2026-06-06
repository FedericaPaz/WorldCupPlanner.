using Domain.Entities;
using Domain.Enums;
using DTOs.Cruces;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class CruceService : ICruceService
{
    private readonly ICrucesRepository _repository;
    private readonly IFixtureService _fixtureService;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly ICrucesGeneratorService _generator;
    private readonly ISimulationService _simulationService;
    private readonly TournamentConfig _config;
    private readonly IMatchRepository _matchRepository;

    public CruceService(ICrucesRepository repository, IFixtureService fixtureService, IStadiumRepository stadiumRepository, ICrucesGeneratorService generator, ISimulationService simulationService, TournamentConfig config, IMatchRepository matchRepository)
    {
        _repository = repository;
        _fixtureService = fixtureService;
        _stadiumRepository = stadiumRepository;
        _generator = generator;
        _simulationService = simulationService;
        _config = config;
        _matchRepository = matchRepository;
    }

    public void GenerateRoundOf32()
    {
        Fixture fixture = _fixtureService.GetFixtureEntity();
        if (fixture == null)
            throw new ArgumentNullException(nameof(fixture));
    
        if (!fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32))
            throw new InvalidOperationException("Todos los partidos de la fase de grupos deben estar jugados antes de generar los cruces.");
    
        foreach (var group in fixture.Groups)
        {
            foreach (var match in group.MatchesInGroup)
                match.Lock();
        }
    
        List<Stadium> estadios = _stadiumRepository.GetAllStadiums();
        int nextMatchId = _matchRepository.GetNextId();
        List<Cruce> cruces = _generator.GenerateRoundOf32(fixture, estadios, _config.SemillaCrucesFase, nextMatchId);
    
        _repository.RemoveCruces();
    
        foreach (var cruce in cruces)
            _matchRepository.AddMatch(cruce.Match);  
    
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
        if (ultimaRonda.Any(c => c.GetWinner() == null))
            throw new InvalidOperationException("Todos los cruces de la ronda actual deben tener resultado antes de generar la siguiente.");
        int nextMatchId = _matchRepository.GetNextId();
        List<Cruce> nuevos = _generator.GenerateNextRound(ultimaRonda, _stadiumRepository.GetAllStadiums(), _config.SemillaCrucesFase, nextMatchId);
        foreach (var cruce in nuevos)
            _matchRepository.AddMatch(cruce.Match); 

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
}