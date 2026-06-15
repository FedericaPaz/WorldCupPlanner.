using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Cruces;
using IRepository;
using IServices.CrucesInterface;
using IServices;
using IServices.FixtureInterfaces;
using Services.Mappers;
using Services.Simulation;

namespace Services;

public class CruceService : ICruceEditService, ICruceQueryService
{
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;

    private readonly ICrucesRepository _repository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly TournamentConfig _config;
    private readonly IMatchRepository _matchRepository;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly IDateTimeMatch _clock;
    private readonly ISimulationEngineFactory _engineFactory;

    private int _nextMatchId = 1;

    public CruceService(
        ICrucesRepository repository,
        IStadiumRepository stadiumRepository,
        TournamentConfig config,
        IMatchRepository matchRepository,
        IFixtureRepository fixtureRepository,
        IDateTimeMatch clock,
        ISimulationEngineFactory engineFactory)
    {
        _repository = repository;
        _stadiumRepository = stadiumRepository;
        _config = config;
        _matchRepository = matchRepository;
        _fixtureRepository = fixtureRepository;
        _clock = clock;
        _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
    }

    public void GenerateRoundOf32()
    {
        try
        {
            var fixture = _fixtureRepository.GetFixture()
                          ?? throw new InvalidOperationException("No hay fixture generado.");
        
            fixture.CargarPartidos(_matchRepository.GetAllMatches());

            if (!fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32))
                throw new InvalidOperationException("Todos los partidos de la fase de grupos deben estar jugados antes de generar los cruces.");

            fixture.LockGroupMatches();

            List<Stadium> estadios = _stadiumRepository.GetAllStadiums();
            int nextMatchId = _matchRepository.GetNextId();
            List<Cruce> cruces = GenerateRoundOf32Internal(fixture, estadios, _config.SemillaCrucesFase, nextMatchId);

            _repository.RemoveCruces();
            foreach (var cruce in cruces)
                _matchRepository.AddMatch(cruce.Match);
            _repository.AddCruces(cruces);
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== ERROR EN GenerateRoundOf32 ===");
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public void GenerateNextRound()
    {
        List<Cruce> actuales = _repository.GetAllCruces();
        if (actuales == null || actuales.Count == 0)
            throw new InvalidOperationException("No hay cruces generados.");

        TournamentPhase ultimaFase = actuales.Max(x => x.Phase);
        if (ultimaFase == TournamentPhase.Final)
            throw new InvalidOperationException("Ya se encuentra en la fase final.");

        List<Cruce> ultimaRonda = actuales.Where(x => x.Phase == ultimaFase).ToList();
        if (ultimaRonda.Any(c => c.GetWinner() == null))
            throw new InvalidOperationException("Todos los cruces de la ronda actual deben tener resultado antes de generar la siguiente.");

        int nextMatchId = _matchRepository.GetNextId();
        List<Cruce> nuevos = GenerateNextRoundInternal(ultimaRonda, _stadiumRepository.GetAllStadiums(), _config.SemillaCrucesFase, nextMatchId);

        foreach (var cruce in nuevos)
            _matchRepository.AddMatch(cruce.Match);
        _repository.AddCruces(nuevos);
    }

    public List<CruceResponse> GetAllCruces()
    {
        return _repository.GetAllCruces().Select(CruceMapper.ToResponse).ToList();
    }

    public void SimulateCruces()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        ISimulationEngine engine = _engineFactory.Create(fixture.SimulationEngineName);
        SimulateCruces(engine);
    }
    
    public void SimulateCruces(ISimulationEngine engine)
    {
        var cruces = _repository.GetAllCruces();
        foreach (var cruce in cruces.Where(c => c.GetWinner() == null))
        {
            var match = cruce.Match;
            var (homeGoals, awayGoals) = CalculateResult(match, _config.SemillaSimulation, engine);

            if (homeGoals == awayGoals)
            {
                var tiebreaker = new Random(_config.SemillaSimulation + match.IdMatch * 31 + 1);
                if (tiebreaker.NextDouble() > 0.5) homeGoals++;
                else awayGoals++;
            }

            match.SimulateResult(homeGoals, awayGoals);
            _matchRepository.UpdateMatch(match);
        }
    }
    
    private (int homeGoals, int awayGoals) CalculateResult(Match match, int seed, ISimulationEngine engine)
    {
        var matchSeed = seed + match.IdMatch;
        return engine.SimulateMatch(match.HomeTeam.CurrentRanking, match.AwayTeam.CurrentRanking, matchSeed);
    }

    private List<Cruce> GenerateRoundOf32Internal(Fixture fixture, List<Stadium> estadios, int semilla, int startMatchId)
    {
        _nextMatchId = startMatchId;
        ValidateInputs(fixture.Groups.ToList(), estadios);

        var equipoAGrupo = fixture.GetTeamToGroupMap();
        var primeros = fixture.GetPrimeros();
        var segundos = fixture.GetSegundos();
        var mejoresTerceros = fixture.GetTerceros().Take(8).ToList();

        var cruces = new List<Cruce>();
        int indicePartidoGlobal = 0;

        cruces.AddRange(PairAndCreateMatches(
            primeros.Take(8).ToList(), mejoresTerceros,
            estadios, semilla, "A", TournamentPhase.RoundOf32,
            ref indicePartidoGlobal, equipoAGrupo));

        cruces.AddRange(PairAndCreateMatches(
            primeros.Skip(8).Take(4).ToList(), segundos.TakeLast(4).Reverse().ToList(),
            estadios, semilla + 1, "B", TournamentPhase.RoundOf32,
            ref indicePartidoGlobal, equipoAGrupo));

        List<Team> segundosRestantes = new List<Team>();
        if (segundos.Count > 4)
            for (int i = 0; i < segundos.Count - 4; i++)
                segundosRestantes.Add(segundos[i]);

        if (segundosRestantes.Count >= 2)
        {
            cruces.AddRange(PairAndCreateMatches(
                segundosRestantes.Take(segundosRestantes.Count / 2).ToList(),
                segundosRestantes.Skip(segundosRestantes.Count / 2).ToList(),
                estadios, semilla + 2, "B", TournamentPhase.RoundOf32,
                ref indicePartidoGlobal, equipoAGrupo, startMatchNumber: 5));
        }

        foreach (var cruce in cruces)
            fixture.AddEliminatoryMatch(cruce.Match);

        return cruces;
    }

    private List<Cruce> GenerateNextRoundInternal(List<Cruce> crucesAnteriores, List<Stadium> estadios, int semilla, int startMatchId)
    {
        _nextMatchId = startMatchId;
        if (crucesAnteriores == null || crucesAnteriores.Count == 0)
            throw new ArgumentException("Se requieren cruces anteriores para generar la siguiente ronda.");

        DateTime fechaUltimoPartido = crucesAnteriores.Max(c => c.Match.DateMatch);

        if (crucesAnteriores[0].Phase == TournamentPhase.SemiFinals)
            return GenerateSemiFinalsOutput(crucesAnteriores, estadios, semilla, fechaUltimoPartido);

        var ganadores = ObtainWinners(crucesAnteriores);
        var siguienteFase = crucesAnteriores[0].GetNextPhase();
        return CreateMatchesForNextPhase(ganadores, estadios, semilla, siguienteFase, fechaUltimoPartido);
    }

    private List<Cruce> PairAndCreateMatches(
        List<Team> lista1,
        List<Team> lista2,
        List<Stadium> estadios,
        int semilla,
        string matchPrefix,
        TournamentPhase fase,
        ref int indicePartidoGlobal,
        Dictionary<string, char>? equipoAGrupo = null,
        int startMatchNumber = 1)
    {
        var lista1Barajada = _config.Shuffle(lista1, semilla);
        var lista2Barajada = _config.Shuffle(lista2, semilla + 7);
        var disponibles = new List<Team>(lista2Barajada);
        var cruces = new List<Cruce>();
        int numeroCruce = startMatchNumber;

        foreach (var equipo1 in lista1Barajada)
        {
            Team? emparejado = null;
            int indiceEmparejado = -1;

            for (int i = 0; i < disponibles.Count; i++)
            {
                bool mismoGrupo = equipoAGrupo != null && AreFromSameGroup(equipo1, disponibles[i], equipoAGrupo);
                if (!mismoGrupo)
                {
                    emparejado = disponibles[i];
                    indiceEmparejado = i;
                    break;
                }
            }

            if (emparejado == null && disponibles.Count > 0)
            {
                emparejado = disponibles[0];
                indiceEmparejado = 0;
            }

            if (emparejado == null)
                throw new InvalidOperationException($"No se pudo emparejar al equipo {equipo1.Name}.");

            var estadio = estadios[indicePartidoGlobal % estadios.Count];
            var partido = CreateMatch(equipo1, emparejado, estadio, indicePartidoGlobal, estadios.Count);
            cruces.Add(new Cruce($"{matchPrefix}{numeroCruce}", partido, fase));
            disponibles.RemoveAt(indiceEmparejado);
            indicePartidoGlobal++;
            numeroCruce++;
        }
        return cruces;
    }

    private static bool AreFromSameGroup(Team equipo1, Team equipo2, Dictionary<string, char> equipoAGrupo)
    {
        if (!equipoAGrupo.TryGetValue(equipo1.Name, out var grupo1)) return false;
        if (!equipoAGrupo.TryGetValue(equipo2.Name, out var grupo2)) return false;
        return grupo1 == grupo2;
    }

    private List<Cruce> CreateMatchesForNextPhase(List<Team> ganadores, List<Stadium> estadios, int semilla, TournamentPhase fase, DateTime ultimaFecha)
    {
        if (ganadores.Count < 2)
            throw new ArgumentException("Se requieren al menos 2 ganadores para crear cruces.");

        var ganadoresBarajados = _config.Shuffle(ganadores, semilla);
        var cruces = new List<Cruce>();
        int indicePartidoGlobal = 0;
        int numeroCruce = 1;

        string prefijo = fase switch
        {
            TournamentPhase.RoundOf16 => "C",
            TournamentPhase.QuarterFinals => "D",
            TournamentPhase.SemiFinals => "S",
            TournamentPhase.Final => "F",
            TournamentPhase.ThirdPlace => "TP",
            _ => "X"
        };

        for (int i = 0; i + 1 < ganadoresBarajados.Count; i += 2)
        {
            var estadio = estadios[indicePartidoGlobal % estadios.Count];
            var partido = CreateMatch(ganadoresBarajados[i], ganadoresBarajados[i + 1], estadio, indicePartidoGlobal, estadios.Count, ultimaFecha);
            cruces.Add(new Cruce($"{prefijo}{numeroCruce}", partido, fase));
            indicePartidoGlobal++;
            numeroCruce++;
        }
        return cruces;
    }

    private Match CreateMatch(Team local, Team visitante, Stadium estadio, int indicePartidoGlobal, int cantEstadios, DateTime? ultimaFecha = null)
    {
        int slotIndex = indicePartidoGlobal / cantEstadios;
        int slotEnDia = slotIndex % _config.MaxPartidosPorDia;
        int diaOffset = slotIndex / _config.MaxPartidosPorDia;
        int hour = StartHour + slotEnDia * HoursBetweenMatches;

        DateTime startDate = ultimaFecha.HasValue
            ? ultimaFecha.Value.Date.AddDays(_config.SeparacionEntreFechas)
            : _config.FechaInicioTorneo.AddDays(21).Date;

        var matchDate = startDate.AddDays(diaOffset).AddHours(hour);
        return new Match(_nextMatchId++, matchDate, estadio, local, visitante, 'N', _clock);
    }

    private static List<Team> ObtainWinners(List<Cruce> cruces)
    {
        var ganadores = new List<Team>();
        foreach (var cruce in cruces)
        {
            var ganador = (Team?)cruce.GetWinner();
            if (ganador == null)
                throw new InvalidOperationException($"El cruce {cruce.Name} no tiene un ganador definido.");
            ganadores.Add(ganador);
        }
        return ganadores;
    }

    private List<Cruce> GenerateSemiFinalsOutput(List<Cruce> semis, List<Stadium> estadios, int semilla, DateTime fechaBase)
    {
        if (semis.Count != 2)
            throw new InvalidOperationException("Se esperan exactamente 2 semifinales.");

        var ganadores = ExtractTeamsFromMatches(semis, c => (Team?)c.GetWinner());
        var perdedores = ExtractTeamsFromMatches(semis, c => (Team?)c.GetLoser());

        int indicePartido = 0;

        var estadioTercero = estadios[indicePartido % estadios.Count];
        var partidoTercero = CreateMatch(perdedores[0], perdedores[1], estadioTercero, indicePartido, estadios.Count, fechaBase);
        var cruceTercero = new Cruce("TP1", partidoTercero, TournamentPhase.ThirdPlace);
        indicePartido++;

        var estadioFinal = estadios[indicePartido % estadios.Count];
        var partidoFinal = CreateMatch(ganadores[0], ganadores[1], estadioFinal, indicePartido, estadios.Count, fechaBase);
        var cruceFinal = new Cruce("F1", partidoFinal, TournamentPhase.Final);

        return new List<Cruce> { cruceTercero, cruceFinal };
    }

    private static List<Team> ExtractTeamsFromMatches(List<Cruce> cruces, Func<Cruce, Team?> teamExtractor)
    {
        var teams = new List<Team>();
        foreach (var cruce in cruces)
        {
            var team = teamExtractor(cruce);
            if (team == null)
                throw new InvalidOperationException($"El cruce {cruce.Name} no tiene un participante definido.");
            teams.Add(team);
        }
        return teams;
    }

    private static void ValidateInputs(List<Group> grupos, List<Stadium> estadios)
    {
        if (grupos == null) throw new ArgumentNullException(nameof(grupos));
        if (estadios == null) throw new ArgumentNullException(nameof(estadios));
        if (grupos.Count == 0) throw new ArgumentException("La lista de grupos no puede estar vacía.");
        if (estadios.Count == 0) throw new ArgumentException("Se requiere al menos un estadio para generar cruces.");
        if (grupos.Count != 12) throw new ArgumentException("El torneo requiere exactamente 12 grupos (A-L).");
    }
}