using Domain.Enums;
using Domain.Entities;

namespace DomainTests;

[TestClass]
public class MatchIncidentTests
{
    [TestMethod]
    public void MatchIncidentWithValidData_CreatingMatchIncident_StoresIncidentType()
    {
        var confederation = new Confederation("UEFA");
        var team = new Team("Uruguay", confederation, 1500);
        var incident = new MatchIncident(IncidentType.YellowCard, team, 1);
        Assert.AreEqual(IncidentType.YellowCard, incident.Type);
    }
}