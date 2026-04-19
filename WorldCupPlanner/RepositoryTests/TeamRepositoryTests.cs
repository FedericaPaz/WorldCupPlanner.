using Domain;
using Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepositoryTests;

[TestClass]
public class TeamRepositoryTests
{   
    [TestMethod]
    public void AddTeam_ShouldStoreTeam()
    {
        var repository = new TeamRepository();
        var conf = new Confederation("AFC");
        var team = new Team("Uruguay", conf, 500);

        repository.Add(team);

        var teams = repository.GetAll();

        Assert.AreEqual(1, teams.Count);
        Assert.AreEqual(team, teams[0]);
    }

    [TestMethod]
    public void Add_OneTeam_ShouldNotThrowException()
    {
        var teamRepository = new TeamRepository();
        var confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 500);
        teamRepository.Add(team);
    }
    
    [TestMethod]
    public void AddTeam_Null_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        Assert.ThrowsException<ArgumentNullException>(() => teamRepository.Add(null));
    }
    
    [TestMethod]
    public void Get_ExistingTeam_ShouldReturnTeam()
    {
        var teamRepository = new TeamRepository();
        var confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 500);
        teamRepository.Add(team);
        var result = teamRepository.Get("Uruguay");
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void Get_NonExistingTeam_ShouldReturnNull()
    {
        var teamRepository = new TeamRepository();
        var result = teamRepository.Get("Argentina");
        Assert.IsNull(result);
    }
}