using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using Domain.Entities;
using DTOs.Team;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class TeamServiceTests
{
    private WorldCupPlannerContext _context;
    private TeamService _teamService;
    private TeamRepository _teamRepository;
    private TournamentConfig _config;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);
        _teamRepository = new TeamRepository(_context);
        _config = new TournamentConfig();
        _teamService = new TeamService(_teamRepository, new FixtureRepository(_context), new MatchRepositoryStub(), new CrucesRepositoryStub(), _config);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private TeamDTO CreateRequest(string name, string confederationName, int fifaRanking)
    {
        return new TeamDTO { Name = name, ConfederationName = confederationName, FifaRanking = fifaRanking };
    }
    
    [TestMethod]
    public void CreateTeam_WithValidData_ShouldAddTeam()
    {
        var request = CreateRequest("Uruguay", "UEFA", 1000);
        _teamService.Add(request);
        var result = _teamRepository.Get("Uruguay");
        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay", result.Name);
        Assert.AreEqual("UEFA", result.Confederation.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }
    
    [TestMethod]
    public void CreateTeam_WithDuplicateName_ShouldThrowException()
    {
        var request = CreateRequest("Uruguay", "UEFA", 1000);
        _teamService.Add(request);
        var duplicatedRequest = CreateRequest("Uruguay", "UEFA", 1001);
        Assert.ThrowsException<ArgumentException>(() => _teamService.Add(duplicatedRequest));
    }
    
    [DataTestMethod]
    [DataRow("UEFA", 16)]
    [DataRow("CONMEBOL", 7)]
    [DataRow("CONCACAF", 7)]
    [DataRow("CAF", 9)]
    [DataRow("AFC", 8)]
    [DataRow("OFC", 1)]
    public void CreateTeam_WhenConfederationLimitExceeded_ShouldThrowException(string confName, int limit)
    {
        for (int i = 0; i < limit; i++)
            _teamService.Add(CreateRequest($"Team{i}", confName, 1000 + i));
        Assert.ThrowsException<InvalidOperationException>(() => _teamService.Add(CreateRequest("ExtraName", confName, 2000)));
    }
    
    [DataTestMethod]
    [DataRow("UEFA", 16)]
    [DataRow("CONMEBOL", 7)]
    [DataRow("CONCACAF", 7)]
    [DataRow("CAF", 9)]
    [DataRow("AFC", 8)]
    [DataRow("OFC", 1)]
    public void CreateTeam_AtConfederationLimit_ShouldSucceed(string confName, int limit)
    {
        for (int i = 0; i < limit; i++)
            _teamService.Add(CreateRequest($"Team{i}", confName, 1000 + i));
        var teams = _teamRepository.GetAll().Count(t => t.Confederation.Name == confName);
        Assert.AreEqual(limit, teams);
    }
    
    [TestMethod]
    public void Get_WhenTeamExists_ShouldReturnTeam()
    {
        var request = CreateRequest("Uruguay", "UEFA", 1000);
        _teamService.Add(request);
        var result = _teamService.Get("Uruguay");
        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay", result.Name);
        Assert.AreEqual("UEFA", result.ConfederationName);
        Assert.AreEqual(1000, result.FifaRanking);
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnAllTeams()
    {
        _teamService.Add(CreateRequest("Brazil", "UEFA", 850));
        _teamService.Add(CreateRequest("Argentina", "UEFA", 750));
        var results = _teamService.GetAll();
        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(t => t.Name == "Brazil"));
        Assert.IsTrue(results.Any(t => t.Name == "Argentina"));
    }
    
    [TestMethod]
    public void Update_WhenTeamExists_ShouldUpdateTeam()
    {
        var originalTeam = new TeamDTO { Name = "Uruguay", ConfederationName = "AFC", FifaRanking = 500 };
        _teamService.Add(originalTeam);
        var updatedTeam = new TeamDTO { Name = "Uruguay", ConfederationName = "CONMEBOL", FifaRanking = 1000 };
        _teamService.Update("Uruguay", updatedTeam);
        var result = _teamService.Get("Uruguay");
        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay", result.Name);
        Assert.AreEqual("CONMEBOL", result.ConfederationName);
        Assert.AreEqual(1000, result.FifaRanking);
    }

    [TestMethod]
    public void Update_WhenTeamDoesNotExist_ShouldThrowException()
    {
        var updatedTeam = new TeamDTO { Name = "Uruguay", ConfederationName = "CONMEBOL", FifaRanking = 1000 };
        Assert.ThrowsException<InvalidOperationException>(() => _teamService.Update("Uruguay", updatedTeam));
    }
    
    [TestMethod]
    public void Delete_WhenTeamExists_ShouldRemoveTeam()
    {
        _teamService.Add(CreateRequest("Brazil", "UEFA", 850));
        _teamService.Delete("Brazil");
        var result = _teamService.Get("Brazil");
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void Delete_WhenTeamDoesNotExist_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _teamService.Delete("NonExistent"));
    }
    
    [TestMethod]
    public void GetAllConfederations_ShouldReturnAllSixConfederations()
    {
        List<ConfederationDTO> result = _teamService.GetAllConfederations();
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
        List<ConfederationDTO> result = _teamService.GetAllConfederations();
        ConfederationDTO? uefa = result.Find(c => c.Name == "UEFA");
        Assert.IsNotNull(uefa);
        Assert.AreEqual(16, uefa.MaxTeams);
        ConfederationDTO? ofc = result.Find(c => c.Name == "OFC");
        Assert.IsNotNull(ofc);
        Assert.AreEqual(1, ofc.MaxTeams);
    }
    
    [TestMethod]
    public void Add_WithNullRequest_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _teamService.Add(null!));
    }

    [TestMethod]
    public void Add_WithEmptyName_ShouldThrowArgumentException()
    {
        var request = CreateRequest("", "UEFA", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Add(request));
    }
    
    [TestMethod]
    public void Add_WithEmptyConfederation_ShouldThrowArgumentException()
    {
        var request = CreateRequest("Uruguay", "", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Add(request));
    }
    
    [TestMethod]
    public void Add_WithInvalidConfederation_ShouldThrowArgumentException()
    {
        var request = CreateRequest("Uruguay", "INVALIDA", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Add(request));
    }
    
    [TestMethod]
    public void Get_WhenTeamDoesNotExist_ShouldReturnNull()
    {
        var result = _teamService.Get("Uruguay");

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void Update_WithEmptyCurrentName_ShouldThrowArgumentException()
    {
        var request = CreateRequest("Uruguay", "CONMEBOL", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Update("", request));
    }
    
    [TestMethod]
    public void Update_WithNullRequest_ShouldThrowArgumentNullException()
    {
        _teamService.Add(CreateRequest("Uruguay", "CONMEBOL", 1000));

        Assert.ThrowsException<ArgumentNullException>(() => _teamService.Update("Uruguay", null!));
    }
    
    [TestMethod]
    public void Update_WithEmptyNewName_ShouldThrowArgumentException()
    {
        _teamService.Add(CreateRequest("Uruguay", "CONMEBOL", 1000));

        var request = CreateRequest("", "CONMEBOL", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Update("Uruguay", request));
    }
    
    [TestMethod]
    public void Update_WithEmptyNewConfederation_ShouldThrowArgumentException()
    {
        _teamService.Add(CreateRequest("Uruguay", "CONMEBOL", 1000));

        var request = CreateRequest("Uruguay", "", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Update("Uruguay", request));
    }
    
    [TestMethod]
    public void Update_WithNewName_ShouldRenameTeam()
    {
        _teamService.Add(CreateRequest("Uruguay", "CONMEBOL", 1000));

        var request = CreateRequest("Uruguay Nuevo", "CONMEBOL", 1000);

        _teamService.Update("Uruguay", request);

        Assert.IsNull(_teamService.Get("Uruguay"));

        var result = _teamService.Get("Uruguay Nuevo");

        Assert.IsNotNull(result);
        Assert.AreEqual("Uruguay Nuevo", result.Name);
    }

    [TestMethod]
    public void Update_WithExistingNewName_ShouldThrowArgumentException()
    {
        _teamService.Add(CreateRequest("Uruguay", "CONMEBOL", 1000));
        _teamService.Add(CreateRequest("Argentina", "CONMEBOL", 1001));

        var request = CreateRequest("Argentina", "CONMEBOL", 1000);

        Assert.ThrowsException<ArgumentException>(() => _teamService.Update("Uruguay", request));
    }

    [TestMethod]
    public void GenerateMissingTeams_WhenRepositoryIsEmpty_ShouldGenerate48Teams()
    {
        _teamService.GenerateMissingTeams();

        var result = _teamService.GetAll();

        Assert.AreEqual(48, result.Count);
    }
    
    [TestMethod]
    public void GenerateMissingTeams_WhenRepositoryAlreadyHas48Teams_ShouldNotAddMoreTeams()
    {
        _teamService.GenerateMissingTeams();

        _teamService.GenerateMissingTeams();

        var result = _teamService.GetAll();

        Assert.AreEqual(48, result.Count);
    }

    [TestMethod]
    public void GenerateMissingTeams_ShouldRespectConfederationLimits()
    {
        _teamService.GenerateMissingTeams();

        var result = _teamRepository.GetAll();

        Assert.AreEqual(16, result.Count(t => t.Confederation.Name == "UEFA"));
        Assert.AreEqual(7, result.Count(t => t.Confederation.Name == "CONMEBOL"));
        Assert.AreEqual(7, result.Count(t => t.Confederation.Name == "CONCACAF"));
        Assert.AreEqual(9, result.Count(t => t.Confederation.Name == "CAF"));
        Assert.AreEqual(8, result.Count(t => t.Confederation.Name == "AFC"));
        Assert.AreEqual(1, result.Count(t => t.Confederation.Name == "OFC"));
    }
}