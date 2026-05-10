using Domain;


namespace Services;

public class CruceGeneratorService
{
    public List<Cruce> GenerateCruces(
        List<Group> grupos,
        List<Stadium> estadios,
        int semilla)
    {
        List<Cruce> cruces = new List<Cruce>();

        Team equipo1 = grupos[0].TeamsInGroup[0];
        Team equipo2 = grupos[0].TeamsInGroup[1];

        for (int i = 1; i <= 16; i++)
        {
            Match partido = new Match(
                DateTime.Now.AddDays(i),
                estadios[0],
                equipo1,
                equipo2,
                'A',
                new ClockStub(DateTime.Now));

            Cruce cruce = new Cruce($"A{i}", partido, TournamentPhase.RoundOf16);
            cruces.Add(cruce);
        }
        return cruces;
    }
}