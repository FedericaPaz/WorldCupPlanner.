using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class CruceGeneratorServiceTests
{
    private List<Group> Create12GroupsWithTeams()
    {
        var grupos = new List<Group>();
        for (char letra = 'A'; letra <= 'L'; letra++)
        {
            var grupo = new Group(letra);
            for (int i = 0; i < 4; i++)
            {
                var equipo = new Team($"Equipo{letra}{i + 1}", new Confederation("UEFA"), 300 + i);
                grupo.AddTeamToGroup(equipo);
            }
            grupos.Add(grupo);
        }
        return grupos;
    }

    private List<Stadium> CreateBaicStadiums()
    {
        var stadiums = new List<Stadium>();
        stadiums.Add(new Stadium("Estadio 1", "Ciudad 1", "Descripción 1", 50000));
        stadiums.Add(new Stadium("Estadio 2", "Ciudad 2", "Descripción 2", 60000));
        stadiums.Add(new Stadium("Estadio 3", "Ciudad 3", "Descripción 3", 70000));
        stadiums.Add(new Stadium("Estadio 4", "Ciudad 4", "Descripción 4", 80000));
        return stadiums;
    }
    
    [TestMethod]
    public void GenerarCruces_With12Groups_ShouldReturns16Cruces()
    {
        var service = new CruceGeneratorService();
        var grupos = Create12GroupsWithTeams();
        var estadios = CreateBaicStadiums();
        int semilla = 12345;
        var cruces = service.GenerateCruces(grupos, estadios, semilla);
        Assert.AreEqual(16, cruces.Count);
    }
}