using Domain.Entities;
using Domain.Import;

namespace DomainTests;

[TestClass]
public class TeamFactoryTests
{
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldCreateTeam()
    {
        var factory = new TeamFactory();
        Team team = factory.CreateTeam("Uruguay", "CONMEBOL", "1800");
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual("CONMEBOL", team.Confederation.Name);
        Assert.AreEqual(1800, team.FifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithNonNumericRanking_ShouldThrowException()
    {
        var factory = new TeamFactory();
        Assert.ThrowsException<ArgumentException>(() => factory.CreateTeam("Uruguay", "CONMEBOL", "A")); 
    }
}