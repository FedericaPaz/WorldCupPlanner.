using Domain;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class TeamServiceTests
{
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldAddTeam()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confedaration = new Confederation("UEFA");
        
        teamService.Add("Uruguay", confedaration, 1000);
        var result = teamRepository.Get("Uruguay");
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void CreateTeam_WithDuplicateName_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confedaration = new Confederation("UEFA");
        
        teamService.Add("Uruguay", confedaration, 1000);
        Assert.ThrowsException<ArgumentException>(() => teamService.Add("Uruguay", confedaration, 1000));
    }
    
    [DataTestMethod]
    [DataRow("UEFA", 16)]
    [DataRow("CONMEBOL", 7)]
    [DataRow("CONCACAF", 7)]
    [DataRow("CAF", 9)]
    [DataRow("AFC", 8)]
    [DataRow("OFC", 1)]
    public void CreateTeam_WhenConfederationLimitExceeded_ShouldThrowException(
        string confName, int limit)
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation(confName);
        
        for (int i = 0; i < limit; i++)
        {
            teamService.Add($"Team{i}", confederation, 1000 + i);
        }
        Assert.ThrowsException<Exception>(() => teamService.Add("ExtraTeam", confederation, 2000));
    }
    
    [DataTestMethod]
    [DataRow("UEFA", 16)]
    [DataRow("CONMEBOL", 7)]
    [DataRow("CONCACAF", 7)]
    [DataRow("CAF", 9)]
    [DataRow("AFC", 8)]
    [DataRow("OFC", 1)]
    public void CreateTeam_AtConfederationLimit_ShouldSucceed(
        string confName, int limit)
    {
        var repo = new TeamRepository();
        var service = new TeamService(repo);
        var conf = new Confederation(confName);

        for (int i = 0; i < limit; i++)
        {
            service.Add($"Team{i}", conf, 1000 + i);
        }

        var teams = repo.GetAll().Where(t => t.Confederation.Name == confName).Count();
        Assert.AreEqual(limit, teams);
    }

    /*[TestMethod]
    public void AddTeam_WithNullConfederation_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation(null);
        Assert.ThrowsException<ArgumentNullException>(() => teamService.Create("Uruguay", confederation, 1000));
    }*/
    
    [TestMethod]
    public void Get_WhenTeamExists_ShouldReturnTeam()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation("UEFA");
        teamService.Add("Brazil", confederation, 1000);
        
        var result = teamService.Get("Brazil");
        Assert.IsNotNull(result);
        Assert.AreEqual("Brazil", result.Name);
    }
    
    /*[TestMethod]
    public void Get_WhenTeamDoesNotExist_ShouldReturnNull()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);

        var result = teamService.Get("Brazil");
        Assert.IsNull(result);
    }*/
    
    [TestMethod]
    public void GetAll_ShouldReturnAllTeams()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation("UEFA");
        teamService.Add("Brazil", confederation, 850);
        teamService.Add("Argentina", confederation, 750);

        var results = teamService.GetAll();
        Assert.AreEqual(2, results.Count);
    }
    
    /*[TestMethod]
    public void Update_WhenTeamExists_ShouldUpdateTeam()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation("UEFA");
        teamService.Add("Brazil", confederation, 850);
        var updatedConfederation = new Confederation("CONMEBOL");
        teamService.Update("Brazil", updatedConfederation, 1000);
        var result = teamService.Get("Brazil");
        Assert.IsNotNull(result);
        Assert.AreEqual("CONMEBOL", result.Confederation.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }*/
    
    [TestMethod]
    public void Delete_WhenTeamExists_ShouldRemoveTeam()
    {
        var repository = new TeamRepository();
        var service = new TeamService(repository);
        var confederation = new Confederation("UEFA");
        service.Add("Brazil", confederation, 850);
        service.Delete("Brazil");
        var result = service.Get("Brazil");
        Assert.IsNull(result);
    }
}