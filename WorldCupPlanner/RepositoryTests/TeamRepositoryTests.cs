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
    
    [TestMethod]
    public void GivenMultipleTeams_WhenGetAll_ShouldReturnAllTeams()
    {
        var teamRepository = new TeamRepository();
        var confederation = new Confederation("AFC");
        var team1 = new Team("Uruguay", confederation, 500);
        var team2 = new Team("Argentina", confederation, 400);
        teamRepository.Add(team1);
        teamRepository.Add(team2);
        var teams = teamRepository.GetAll();
        Assert.AreEqual(2, teams.Count);
        Assert.IsTrue(teams.Contains(team1));
        Assert.IsTrue(teams.Contains(team2));
    }
    
    [TestMethod]
    public void DeleteTeam_WhenExists_ShouldRemoveTeam()
    {
        var teamRepository = new TeamRepository();
        var confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 500);

        teamRepository.Add(team);
        teamRepository.Delete(team);

        var result = teamRepository.GetAll();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void DeleteTeam_WhenNonExists_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 500);
        Assert.ThrowsException<ArgumentException>(() => teamRepository.Delete(team));
    }
    
    [TestMethod]
    public void UpdateTeam_WhenExists_ShouldUpdateTeam()
    {
        var teamRepository = new TeamRepository();
        var  confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 500);
        teamRepository.Add(team);

        var updatedTeam = new Team(team.Name, team.Confederation, 1000);
        teamRepository.Update(updatedTeam);

        var result = teamRepository.Get(team.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }

    [TestMethod]
    public void UpdateTeam_WhenNonExists_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var confederation = new Confederation("AFC");
        var team = new Team("Uruguay", confederation, 500);
        Assert.ThrowsException<ArgumentException>(() => teamRepository.Update(team));
    }    
    
}