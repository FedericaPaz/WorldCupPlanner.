using DTOs;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class TeamServiceTests
{
    private TeamDTO CreateRequest(string name, string confederationName, int fifaRanking)
    {
        return new TeamDTO()
        {
            Name = name,
            ConfederationName = confederationName,
            FifaRanking = fifaRanking
        };
    }
    
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldAddTeam()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var request = CreateRequest("Uruguay", "UEFA", 1000);
        
        teamService.Add(request);
        var result = teamRepository.Get("Uruguay");
        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay", result.Name);
        Assert.AreEqual("UEFA", result.Confederation.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }
    
    [TestMethod]
    public void CreateTeam_WithDuplicateName_ShouldThrowException()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        var request = CreateRequest("Uruguay", "UEFA", 1000);
        
        teamService.Add(request);
        Assert.ThrowsException<ArgumentException>(() => teamService.Add(request));
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

        for (int i = 0; i < limit; i++)
        {
            var request = CreateRequest($"Team{i}", confName, 1000 + i);
            teamService.Add(request);
        }
        var extraRequest = CreateRequest("ExtraName", confName, 2000);
        Assert.ThrowsException<InvalidOperationException>(() => teamService.Add(extraRequest));
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

        for (int i = 0; i < limit; i++)
        {
            var request = CreateRequest($"Team{i}", confName, 1000 + i);
            service.Add(request);
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
        var request = CreateRequest("Uruguay", "UEFA", 1000);
        teamService.Add(request);
        
        var result = teamService.Get("Uruguay");
        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay", result.Name);
        Assert.AreEqual("UEFA", result.ConfederationName);
        Assert.AreEqual(1000, result.FifaRanking);
    }
    
    /*[TestMethod]
    public void Get_WhenTeamDoesNotExist_ShouldReturnNull()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);

        var result = teamService.Get("NonExistent");
        Assert.IsNull(result);
    }*/
    
    [TestMethod]
    public void GetAll_ShouldReturnAllTeams()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        teamService.Add(CreateRequest("Brazil", "UEFA", 850));
        teamService.Add(CreateRequest("Argentina", "UEFA", 750));

        var results = teamService.GetAll();
        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(t => t.Name == "Brazil"));
        Assert.IsTrue(results.Any(t => t.Name == "Argentina"));
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
        service.Add(CreateRequest("Brazil", "UEFA", 850));
        service.Delete("Brazil");
        var result = service.Get("Brazil");
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void Delete_WhenTeamDoesNotExist_ShouldThrowException()
    {
        var repository = new TeamRepository();
        var service = new TeamService(repository);
        Assert.ThrowsException<InvalidOperationException>(() => service.Delete("NonExistent"));
    }
    
    [TestMethod]
    public void GetAllConfederations_ShouldReturnAllSixConfederations()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        List<ConfederationDTO> result = teamService.GetAllConfederations();
        Assert.AreEqual(6, result.Count);
        Assert.IsTrue(result.Any(c => c.Name == "UEFA"));
        Assert.IsTrue(result.Any(c => c.Name == "CONMEBOL"));
        Assert.IsTrue(result.Any(c => c.Name == "CONCACAF"));
        Assert.IsTrue(result.Any(c => c.Name == "CAF"));
        Assert.IsTrue(result.Any(c => c.Name == "AFC"));
        Assert.IsTrue(result.Any(c => c.Name == "OFC"));
    }
    
    [TestMethod]
    public void GetAllConfederations_ShouldReturnCorrectMaxTeams()
    {
        var teamRepository = new TeamRepository();
        var teamService = new TeamService(teamRepository);
        List<ConfederationDTO> result = teamService.GetAllConfederations();
        ConfederationDTO? uefa = result.Find(c => c.Name == "UEFA");
        Assert.IsNotNull(uefa);
        Assert.AreEqual(16, uefa.MaxTeams);
        ConfederationDTO? ofc = result.Find(c => c.Name == "OFC");
        Assert.IsNotNull(ofc);
        Assert.AreEqual(1, ofc.MaxTeams);
    }
}