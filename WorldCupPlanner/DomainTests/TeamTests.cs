using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DomainTests;

[TestClass]
public class TeamTests
{
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", "AFC", 301);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual("AFC", team.Confederation);
        Assert.AreEqual(301, team.FifaRanking);
    }
    
    [TestMethod]
    public void CreateTeam_WithInvalidName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", "AFC", 301));
    }

    [TestMethod]
    public void CreateTeam_WithEmptyConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", "", 301));
    }
}