using Domain;

namespace Services;

public class CruceGeneratorService
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
    }
    
    private List<Cruce> CrearCruces(List<Team> equipos, List<Stadium> estadios, int semilla, TournamentPhase fase)
    {
        Random random = new Random(semilla);
        equipos = equipos.OrderBy(x => random.Next()).ToList();
        List<Cruce> cruces = new List<Cruce>();
        int indiceEstadio = 0;
        for (int i = 0; i < equipos.Count; i += 2)
        {
            Team local = equipos[i];
            Team visitante = equipos[i + 1];
            Stadium estadio = estadios[indiceEstadio];
            Match partido = new Match(DateTime.Now.AddDays(i), estadio, local, visitante, 'A', new ClockStub(DateTime.Now));
            Cruce cruce = new Cruce($"C{(i / 2) + 1}", partido, fase);
            cruces.Add(cruce);
            indiceEstadio++;
            if (indiceEstadio >= estadios.Count)
                indiceEstadio = 0;
        }
        return cruces;
    }
}