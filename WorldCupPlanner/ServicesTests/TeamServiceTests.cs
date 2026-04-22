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
}