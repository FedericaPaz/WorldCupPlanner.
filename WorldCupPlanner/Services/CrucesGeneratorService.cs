using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using IServices;

namespace Services;

public class CrucesGeneratorService : ICrucesGeneratorService
{
    private readonly TournamentConfig _config;
    private readonly IDateTimeMatch _clock;

    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;
    
    private int _nextMatchId = 1;
    public CrucesGeneratorService(TournamentConfig config, IDateTimeMatch clock)
    {
        _config = config;
        _clock = clock;
        _nextMatchId  = 1;
    }

    public List<Cruce> GenerateRoundOf32(Fixture fixture, List<Stadium> estadios, int semilla, int startMatchId = 1)
    {
        _nextMatchId = startMatchId;
        var grupos = fixture.Groups.ToList();
        ValidateInputs(grupos, estadios);

        var equipoAGrupo = BuildTeamToGroupMap(grupos);
        var (primeros, segundos, mejoresTerceros) = GetClassifiedTeamsByCategory(grupos);
        var cruces = new List<Cruce>();

        int indicePartidoGlobal = 0;
        
        // A1-A8: 8 primeros con mayor puntaje vs 8 mejores terceros
        cruces.AddRange(PairAndCreateMatches(
            primeros.Take(8).ToList(),
            mejoresTerceros,
            estadios, semilla, "A", TournamentPhase.RoundOf32,
            ref indicePartidoGlobal, equipoAGrupo));

        // B1-B4: 4 primeros restantes vs 4 segundos con menor puntaje
        cruces.AddRange(PairAndCreateMatches(
            primeros.Skip(8).Take(4).ToList(),
            segundos.TakeLast(4).Reverse().ToList(),
            estadios, semilla + 1, "B", TournamentPhase.RoundOf32,
            ref indicePartidoGlobal, equipoAGrupo));
        
        // B5-B8: 8 segundos restantes entre sí
        List<Team> segundosRestantes = new List<Team>();
        if (segundos.Count > 4)
        {
            for (int i = 0; i < segundos.Count - 4; i++)
                segundosRestantes.Add(segundos[i]);
        }

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
    public List<Cruce> GenerateNextRound(List<Cruce> crucesAnteriores, List<Stadium> estadios, int semilla, int startMatchId = 1)
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

    private (List<Team> primeros, List<Team> segundos, List<Team> mejoresTerceros) GetClassifiedTeamsByCategory(List<Group> grupos)
    {
        var primerosPorGrupo = new List<Standing>();
        var segundosPorGrupo = new List<Standing>();
        var tercerosPorGrupo = new List<Standing>();

        foreach (var grupo in grupos)
        {
            var standings = grupo.GetStandings();
            if (standings.Count >= 3)
            {
                primerosPorGrupo.Add(standings[0]);
                segundosPorGrupo.Add(standings[1]);
                tercerosPorGrupo.Add(standings[2]);
            }
        }

        var primeros = SortAndBreakTies(primerosPorGrupo);
        var segundos = SortAndBreakTies(segundosPorGrupo);
        var mejoresTerceros = SortAndBreakTies(tercerosPorGrupo).Take(8).ToList();

        return (primeros, segundos, mejoresTerceros);
    }

    private List<Team> SortAndBreakTies(List<Standing> standings)
    {
        var ordenados = standings
            .OrderByDescending(s => s.PointsInGroup)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();

        var resultado = new List<Standing>();
        var tieBreakRandomizer = new Random(_config.SemillaCrucesFase);
        int i = 0;

        while (i < ordenados.Count)
        {
            int j = i + 1;
            while (j < ordenados.Count
                && ordenados[j].PointsInGroup == ordenados[i].PointsInGroup
                && ordenados[j].GoalDifference == ordenados[i].GoalDifference
                && ordenados[j].GoalsFor == ordenados[i].GoalsFor)
            {
                j++;
            }

            var empatados = ordenados.Skip(i).Take(j - i).ToList();
            if (empatados.Count > 1) empatados = ApplyFisherYates(empatados, tieBreakRandomizer);

            resultado.AddRange(empatados);
            i = j;
        }
        return resultado.Select(s => s.TeamInGroup).ToList();
    }

    private static List<T> ApplyFisherYates<T>(List<T> list, Random shuffleRandomizer)
    {
        var arr = list.ToArray();
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = shuffleRandomizer.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr.ToList();
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

            if (emparejado == null)
                throw new InvalidOperationException($"No se pudo emparejar al equipo {equipo1.Name}: todos los disponibles son del mismo grupo.");

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

    private static Dictionary<string, char> BuildTeamToGroupMap(List<Group> grupos)
    {
        var map = new Dictionary<string, char>();
        foreach (var grupo in grupos)
        foreach (var team in grupo.TeamsInGroup)
            map[team.Name] = grupo.GroupLetter;
        return map;
    }

    private List<Cruce> CreateMatchesForNextPhase(List<Team> ganadores, List<Stadium> estadios, int semilla, TournamentPhase fase, DateTime ultimaFecha)
    {
        if (ganadores.Count < 2) throw new ArgumentException("Se requieren al menos 2 ganadores para crear cruces.");

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
        int partidosPorSlot = cantEstadios;
        int slotIndex = indicePartidoGlobal / partidosPorSlot;
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
        if (grupos == null)
            throw new ArgumentNullException(nameof(grupos));
        if (estadios == null)
            throw new ArgumentNullException(nameof(estadios));
        if (grupos.Count == 0)
            throw new ArgumentException("La lista de grupos no puede estar vacía.");
        if (estadios.Count == 0)
            throw new ArgumentException("Se requiere al menos un estadio para generar cruces.");
        if (grupos.Count != 12)
            throw new ArgumentException("El torneo requiere exactamente 12 grupos (A-L).");
    }
}
