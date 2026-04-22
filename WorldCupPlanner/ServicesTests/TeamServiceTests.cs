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
        
        teamService.Create("Uruguay", confedaration, 1000);
        var result = teamRepository.Get("Uruguay");
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void CreateTeam_WithDuplicateName_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confedaration = new Confederation("UEFA");
        
        teamService.Create("Uruguay", confedaration, 1000);
        Assert.ThrowsException<ArgumentException>(() => teamService.Create("Uruguay", confedaration, 1000));
    }
    
    [TestMethod]
    public void CreateTeam_WhenUEFALimitExceeded_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation("UEFA");
        
        for (int i = 0; i < 16; i++)
        {
            teamService.Create($"Team{i}", confederation, 1000 + i);
        }
        
        Assert.ThrowsException<Exception>(() => teamService.Create("ExtraTeam", confederation, 2000));
    }
    
    [TestMethod]
    public void CreateTeam_WhenOFCLimitExceeded_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var confederation = new Confederation("OFC");
        teamService.Create("Team1", confederation, 1000);
        Assert.ThrowsException<Exception>(() => teamService.Create("ExtraTeam", confederation, 2000));
    }
}