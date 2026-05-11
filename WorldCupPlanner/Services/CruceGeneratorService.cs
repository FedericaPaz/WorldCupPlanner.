using Domain;
using IServices;

namespace Services;

public class CruceGeneratorService : ICruceGeneratorService
{
    private readonly StandingCalculator _calculator;

    public CruceGeneratorService()
    {
        _calculator = new StandingCalculator();
    }
    
    public List<Cruce> GenerateRoundOf32(List<Group> grupos, List<Stadium> estadios, int semilla)
    {
        if (grupos == null) throw new ArgumentNullException(nameof(grupos));
        if (grupos.Count == 0)
            throw new ArgumentException("La lista de grupos no puede estar vacía.");
            
        List<Team> clasificados = ObtenerClasificados(grupos);
        return CrearCruces(clasificados, estadios, semilla, TournamentPhase.RoundOf32);
    }
    
    private List<Team> ObtenerClasificados(List<Group> grupos)
    {
        List<Team> clasificados = new List<Team>();
        List<Standing> terceros = new List<Standing>();
        foreach (Group grupo in grupos)
        {
            List<Standing> standings = _calculator.CalculateStandings(grupo);
            clasificados.Add(standings[0].TeamInGroup);
            clasificados.Add(standings[1].TeamInGroup);
            terceros.Add(standings[2]);
        }

        List<Team> mejoresTerceros =
            terceros
                .OrderByDescending(x => x.PointsInGroup)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsFor)
                .Take(8)
                .Select(x => x.TeamInGroup)
                .ToList();

        clasificados.AddRange(mejoresTerceros);
        return clasificados;
    }
    
    private List<Cruce> CrearCruces(List<Team> equipos, List<Stadium> estadios, int semilla, TournamentPhase fase)
    {
        TournamentConfig config = new TournamentConfig();
        equipos = config.Shuffle(equipos, semilla);
        List<Cruce> cruces = new List<Cruce>();
        int indiceEstadio = 0;
        for (int i = 0; i < equipos.Count; i += 2)
        {
            Team local = equipos[i];
            Team visitante = equipos[i + 1];
            Stadium estadio = estadios[indiceEstadio];
            DateTime fechaPartido = DateTime.Now.AddHours(-2);
            DateTime fechaActual  =  DateTime.Now;
            Match partido = new Match(fechaPartido, estadio, local, visitante, 'A', new ClockStub(fechaActual));
            Cruce cruce = new Cruce($"C{(i / 2) + 1}", partido, fase);
            cruces.Add(cruce);
            indiceEstadio++;
            if (indiceEstadio >= estadios.Count)
                indiceEstadio = 0;
        }
        return cruces;
    }

    public List<Cruce> GenerateNextRound(List<Cruce> crucesAnteriores, List<Stadium> estadios, int semilla)
    {
        List<Team> ganadores = ObtainWinners(crucesAnteriores);
        TournamentPhase siguienteFase = crucesAnteriores[0].ObtainNextPhase();
        return CrearCruces(ganadores, estadios, semilla, siguienteFase);
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
