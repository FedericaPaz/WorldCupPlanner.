using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DomainTests;

[TestClass]
public class TeamTests
{
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 301);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(confederation, team.Confederation);
        Assert.AreEqual(301, team.FifaRanking);
    }
    
    [TestMethod]
    public void CreateTeam_WithEmptyName_ShouldThrowException()
    {
        var confederation = new Confederation("AFC");
        Assert.ThrowsException<ArgumentException>(() => new Team("", confederation, 301));
    }

    [TestMethod]
    public void CreateTeam_WithNameTooLong_ShouldThrowException()
    {
        var confederation = new Confederation("AFC");
        Assert.ThrowsException<ArgumentException>(() => new Team("AAAAAAAAAABBBBBBBBBBCCCCCCCCCCDDDDDDDDDDEEEEEEEEEEFFFFFFFFFFG", confederation, 301));
    }

    [TestMethod]
    public void CreateTeam_WithEmptyConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", null, 301));
    }

    [TestMethod]
    public void CreateTeam_WithRankingBelowMinimum_ShouldThrowException()
    {
        var confederation = new Confederation("AFC");
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", confederation, 299));
        
    }

    [TestMethod]
    public void CreateTeam_WithRankingAboveMaximum_ShouldThrowException()
    {
        var confederation = new Confederation("AFC");
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", confederation, 2501));
    }
}