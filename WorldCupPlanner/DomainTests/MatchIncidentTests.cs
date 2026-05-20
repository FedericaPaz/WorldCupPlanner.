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
    
    [TestMethod]
    public void MatchIncidentWithValidData_CreatingMatchIncident_StoresTeam()
    {
        var confederation = new Confederation("UEFA");
        var team = new Team("Uruguay", confederation, 1500);
        var incident = new MatchIncident(IncidentType.YellowCard, team, 1);
        Assert.AreEqual(team, incident.Team);
    }
    
    [TestMethod]
    public void MatchIncidentWithValidData_CreatingMatchIncident_StoresQuantity()
    {
        var confederation = new Confederation("UEFA");
        var team = new Team("Uruguay", confederation, 1500);
        var incident = new MatchIncident(IncidentType.YellowCard, team, 2);
        Assert.AreEqual(2, incident.Quantity);
    }
    
    [TestMethod]
    public void CreateMatchIncident_WithNegativeQuantity_ShouldThrowArgumentException()
    {
        var confederation = new Confederation("UEFA");
        var team = new Team("Uruguay", confederation, 1500);
        Assert.ThrowsException<ArgumentException>(() => new MatchIncident(IncidentType.YellowCard, team, -1));
    }
}