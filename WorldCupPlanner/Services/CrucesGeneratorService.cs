using Domain;
using IServices;

namespace Services;

public class CrucesGeneratorService : ICrucesGeneratorService
{
    private readonly StandingCalculator _calculator;
    private readonly TournamentConfig _config;
    private Dictionary<string, char> _equipoAGrupo = new();
    public CrucesGeneratorService(TournamentConfig config)
    {
        _calculator = new StandingCalculator();
        _config = config;
    }
    
    public List<Cruce> GenerateRoundOf32(List<Group> grupos, List<Stadium> estadios, int semilla)
    {
        ValidateInputs(grupos, estadios);
        _equipoAGrupo.Clear();
        foreach (var grupo in grupos)
        {
            foreach (var team in grupo.TeamsInGroup)
                _equipoAGrupo[team.Name] = grupo.GroupLetter;
        }
        
        var (primeros, segundos, mejoresTerceros) = ObtenerClasificadosPorCategoria(grupos);
        var cruces = new List<Cruce>();
        int indiceEstadio = 0;
        
        //A1-A8: 8 primeros vs 8 mejores terceros
        cruces.AddRange(EmparejaryCrearCruces(
            primeros.Take(8).ToList(),         
            mejoresTerceros, estadios, semilla, "A", ref indiceEstadio));
        
        // B1-B4: 4 primeros restantes vs 4 segundos con menor puntaje
        cruces.AddRange(EmparejaryCrearCruces(
            primeros.Skip(8).Take(4).ToList(), 
            segundos.TakeLast(4).Reverse().ToList(), 
            estadios, semilla + 1, "B", ref indiceEstadio));
        
        // B5-B8: 8 segundos entre sí
        var segundosRestantes = segundos.Count > 4 ? segundos.Take(segundos.Count - 4).ToList() : new List<Team>();
        if (segundosRestantes.Count >= 2)
        {
            cruces.AddRange(EmparejaryCrearCruces(
                segundosRestantes.Take(segundosRestantes.Count / 2).ToList(),
                segundosRestantes.Skip(segundosRestantes.Count / 2).ToList(),
                estadios, semilla + 2, "B", ref indiceEstadio, empezarEn: 5
            ));
        }
        return cruces;
    }
    private (List<Team> primeros, List<Team> segundos, List<Team> mejoresTerceros) ObtenerClasificadosPorCategoria(List<Group> grupos)
    {
        var primerosPorGrupo = new List<Standing>();
        var segundosPorGrupo = new List<Standing>();
        var tercerosPorGrupo = new List<Standing>();
        
        foreach (var grupo in grupos)
        {
            var standings = _calculator.CalculateStandings(grupo);
            if (standings.Count >= 3)
            {
                primerosPorGrupo.Add(standings[0]);
                segundosPorGrupo.Add(standings[1]);
                tercerosPorGrupo.Add(standings[2]);
            }
        }
        
        var primeros = primerosPorGrupo
            .OrderByDescending(s => s.PointsInGroup)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .Select(s => s.TeamInGroup)
            .ToList();
        
        var segundos = segundosPorGrupo
            .OrderByDescending(s => s.PointsInGroup)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .Select(s => s.TeamInGroup)
            .ToList();
        
        var mejoresTerceros = tercerosPorGrupo
            .OrderByDescending(s => s.PointsInGroup)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .Take(8)
            .Select(s => s.TeamInGroup)
            .ToList();
        
        return (primeros, segundos, mejoresTerceros);
    }
    public List<Cruce> GenerateNextRound(List<Cruce> crucesAnteriores, List<Stadium> estadios, int semilla)
    {
        if (crucesAnteriores == null || crucesAnteriores.Count == 0)
            throw new ArgumentException("Se requieren cruces anteriores para generar la siguiente ronda.");
        var ganadores = ObtainWinners(crucesAnteriores);
        var siguienteFase = crucesAnteriores[0].ObtainNextPhase();
        return CrearCrucesProximaFase(ganadores, estadios, semilla, siguienteFase);
    }
    
    
    private List<Cruce> EmparejaryCrearCruces(List<Team> lista1, List<Team> lista2, List<Stadium> estadios, int semilla, string prefijoCruce, ref int indiceEstadioActual, int empezarEn = 1)
    {
        //var config = new TournamentConfig();
        var lista1Barajada = _config.Shuffle(lista1, semilla);
        var lista2Barajada = _config.Shuffle(lista2, semilla ^ 0xFFFF);
        var disponibles = new List<Team>(lista2Barajada);
        var cruces = new List<Cruce>();
        int numeroCruce = empezarEn;
        foreach (var equipo1 in lista1Barajada)
        {
            Team? emparejado = null;
            int indiceEmparejado = -1;
            for (int i = 0; i < disponibles.Count; i++)
            {
                if (!SonDelMismoGrupo(equipo1, disponibles[i])) 
                {
                    emparejado = disponibles[i];
                    indiceEmparejado = i;
                    break;
                }
            }
            if (emparejado != null)
            {
                var partido = CrearPartido(equipo1, emparejado, estadios[indiceEstadioActual % estadios.Count], numeroCruce);
                var nombreCruce = $"{prefijoCruce}{numeroCruce}";
                cruces.Add(new Cruce(nombreCruce, partido, TournamentPhase.RoundOf32));
                disponibles.RemoveAt(indiceEmparejado);
                indiceEstadioActual++;
                numeroCruce++;
            }
        }
        return cruces;
    }
    private bool SonDelMismoGrupo(Team equipo1, Team equipo2)
    {
        if (!_equipoAGrupo.TryGetValue(equipo1.Name, out var grupo1))
            return false;
        if (!_equipoAGrupo.TryGetValue(equipo2.Name, out var grupo2))
            return false;
        return grupo1 == grupo2;
    }
    private Match CrearPartido(Team local, Team visitante, Stadium estadio, int numeroCruce)
    {
        var fechaBase = _config.FechaInicioTorneo.AddDays(21 + numeroCruce);
        var clockCreacion = new ClockStub(_config.FechaInicioTorneo);
        return new Match(fechaBase, estadio, local, visitante, 'N', clockCreacion);
    }
    private List<Cruce> CrearCrucesProximaFase(List<Team> ganadores, List<Stadium> estadios, int semilla, TournamentPhase fase)
    {
        if (ganadores.Count < 2)
            throw new ArgumentException("Se requieren al menos 2 ganadores para crear cruces.");
        var config = new TournamentConfig();
        var ganadoresBarajados = config.Shuffle(ganadores, semilla);
        var cruces = new List<Cruce>();
        int indiceEstadio = 0;
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
        
        for (int i = 0; i < ganadoresBarajados.Count; i += 2)
        {
            if (i + 1 < ganadoresBarajados.Count)
            {
                var partido = CrearPartido(ganadoresBarajados[i], ganadoresBarajados[i + 1], estadios[indiceEstadio % estadios.Count], numeroCruce);
                var nombreCruce = $"{prefijo}{numeroCruce}";
                cruces.Add(new Cruce(nombreCruce, partido, fase));
                indiceEstadio++;
                numeroCruce++;
            }
        }
        return cruces;
    }
    private List<Team> ObtainWinners(List<Cruce> cruces)
    {
        List<Team> ganadores = new List<Team>();
        foreach (Cruce cruce in cruces)
        {
            Team? ganador = (Team?)cruce.GetWinner();
            if (ganador == null)
                throw new InvalidOperationException($"El cruce {cruce.Nombre} no tiene un ganador definido.");
            ganadores.Add(ganador);
        }
        return ganadores;
    }
    private void ValidateInputs(List<Group> grupos, List<Stadium> estadios)
    {
        if (grupos == null)
            throw new ArgumentNullException(nameof(grupos), "La lista de grupos no puede ser nula.");
        if (estadios == null)
            throw new ArgumentNullException(nameof(estadios), "La lista de estadios no puede ser nula.");
        if (grupos.Count == 0)
            throw new ArgumentException("La lista de grupos no puede estar vacía.");
        if (estadios.Count == 0)
            throw new ArgumentException("Se requiere al menos un estadio para generar cruces.");
        if (grupos.Count != 12)
            throw new ArgumentException("El torneo requiere exactamente 12 grupos (A-L).");
    }
}
