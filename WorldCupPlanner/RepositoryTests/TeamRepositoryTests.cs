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
        var team = CreateTeam("Uruguay");
        repository.Add(team);

        var teams = repository.GetAll();

        Assert.AreEqual(1, teams.Count);
        Assert.AreEqual(team, teams[0]);
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
        var team = CreateTeam("Uruguay");
        teamRepository.Add(team);
        var result = teamRepository.Get("Uruguay");
        Assert.AreEqual(team, result);
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
        var team1 = CreateTeam("Uruguay");
        var team2 = CreateTeam("Argentina");
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
        var team = CreateTeam("Uruguay");

        teamRepository.Add(team);
        teamRepository.Delete(team);

        var result = teamRepository.GetAll();

        Assert.IsNull(teamRepository.Get("Uruguay"));
    }

    [TestMethod]
    public void DeleteTeam_WhenNonExists_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var team = CreateTeam("Uruguay");
        Assert.ThrowsException<InvalidOperationException>(() => teamRepository.Delete(team));
    }
    
    [TestMethod]
    public void UpdateTeam_WhenExists_ShouldUpdateTeam()
    {
        var teamRepository = new TeamRepository();
        var team = CreateTeam("Uruguay");
        teamRepository.Add(team);

        var updatedTeam = new Team(team.Name, team.Confederation, 1000);
        teamRepository.Update(updatedTeam);

        var result = teamRepository.Get(team.Name);
        Assert.AreEqual(updatedTeam, result);
    }

    [TestMethod]
    public void UpdateTeam_WhenNonExists_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var team = CreateTeam("Uruguay");
        Assert.ThrowsException<InvalidOperationException>(() => teamRepository.Update(team));
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnCopy_NotInternalList()
    {
        var teamRepository = new TeamRepository();
        var team = CreateTeam("Uruguay");
        teamRepository.Add(team);

        var list = teamRepository.GetAll();
        list.Clear();

        var result = teamRepository.GetAll();
        Assert.AreEqual(1, result.Count);
    }
    
    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("AFC"), 500);
    }
}