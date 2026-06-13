using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using IRepository;
using Repository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class FixtureServiceTests
{
    private WorldCupPlannerContext _context;
    private FixtureRepository _fixtureRepository = null!;
    private FixtureService _service = null!;
    private ITeamRepository _teamRepository = null!;
    private IStadiumRepository _stadiumRepository = null!;
    private TournamentConfig _config = null!;
    private Fixture _fixture = null!;
    private FixtureGenerator _fixtureGenerator = null!;
    private IMatchRepository _matchRepository = null!;
    private ICrucesRepository _crucesRepository = null!;
    private IGroupRepository _groupRepository;
    private static readonly DateTime ClockDate = new DateTime(2026, 1, 1);

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);

        _fixtureRepository = new FixtureRepository(_context);
        _teamRepository = new TeamRepository(_context);

        _stadiumRepository = new StadiumRepositoryStub();
        _matchRepository = new MatchRepositoryStub();
        
        _crucesRepository = new CrucesRepository(_context); 

        _config = new TournamentConfig();
        var clock = new ClockStub(ClockDate);
        _fixtureGenerator = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, clock);
        _groupRepository = new GroupRepository(_context);

        _service = new FixtureService(
            _fixtureRepository, _fixtureGenerator, _teamRepository,
            _stadiumRepository, _matchRepository, _crucesRepository,
            _groupRepository, 
            _config);
    
        _fixture = new Fixture(_config);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        if (_context != null)
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    private Fixture CreateValidFixture()
    {
        var config    = new TournamentConfig();
        var fixture   = new Fixture(config);
        fixture.InitializeGroups();
        var clock     = new ClockStub(ClockDate);
        var matchDate = ClockDate.AddMonths(5);
        int matchId   = 1;

        var stadiumNames = new[] { "Estadio1", "Estadio2", "Estadio3", "Estadio4" };
        foreach (var name in stadiumNames)
            _stadiumRepository.AddStadium(new Stadium(name, "Ciudad", "", 50000));

        foreach (var group in fixture.Groups)
        {
            for (int i = 1; i <= 4; i++)
            {
                var team = new Team($"Team_{group.GroupLetter}_{i}", new Confederation("UEFA"), 1000);
                _teamRepository.Add(team);
                group.AddTeamToGroup(team);
            }

            _groupRepository.AddGroup(group);

            var teams = group.TeamsInGroup.ToList();
            var matchups = new (int, int)[] { (0,1),(0,2),(0,3),(1,2),(1,3),(2,3) };
            for (int i = 0; i < 6; i++)
            {
                var stadium = _stadiumRepository.GetStadium(stadiumNames[i % 4]);
                var match = new Match(matchId++, matchDate, stadium,
                    teams[matchups[i].Item1], teams[matchups[i].Item2],
                    group.GroupLetter, clock);
                group.AddMatchToGroup(match);
                _matchRepository.AddMatch(match);
            }
        }

        _fixtureRepository.AddFixture(fixture);
        return fixture;
    }

    private void AddRequiredTeams()
    {
        AddTeamsForConfederation("UEFA",     16, 1000);
        AddTeamsForConfederation("CONMEBOL",  7, 1200);
        AddTeamsForConfederation("CONCACAF",  7, 1400);
        AddTeamsForConfederation("CAF",       9, 1600);
        AddTeamsForConfederation("AFC",       8, 1800);
        AddTeamsForConfederation("OFC",       1, 2000);
    }

    private void AddTeamsForConfederation(string confederationName, int amount, int rankingBase)
    {
        for (int i = 1; i <= amount; i++)
            _teamRepository.Add(new Team($"{confederationName}_Team_{i}", new Confederation(confederationName), rankingBase + i));
    }

    private void AddRequiredStadiums()
    {
        for (int i = 1; i <= 12; i++)
            _stadiumRepository.AddStadium(new Stadium($"Stadium_{i}", "Montevideo", "", 50000));
    }

    [TestMethod]
    public void HasFixture_WhenNoFixture_ReturnsFalse()
    {
        Assert.IsFalse(_service.HasFixture());
    }

    [TestMethod]
    public void HasFixture_WhenFixtureExists_ReturnsTrue()
    {
        CreateValidFixture();
        Assert.IsTrue(_service.HasFixture());
    }

    [TestMethod]
    public void GetFixture_WhenNoFixture_ReturnsEmptyResponse()
    {
        var result = _service.GetFixture();
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Groups.Count);
        Assert.AreEqual("", result.CurrentPhase);
    }

    [TestMethod]
    public void GetFixture_WhenFixtureExists_ReturnsFixtureResponse()
    {
        CreateValidFixture();
        Assert.IsNotNull(_service.GetFixture());
    }

    [TestMethod]
    public void GetFixture_ReturnsCorrectGroupData()
    {
        CreateValidFixture();
        var groupA = _service.GetFixture().Groups.First(g => g.GroupLetter == 'A');
        Assert.IsNotNull(groupA);
        Assert.AreEqual(4, groupA.Teams.Count);
        Assert.AreEqual(6, groupA.Matches.Count);
        Assert.IsTrue(groupA.Teams.First().Name.StartsWith("Team_A"));
    }

    [TestMethod]
    public void GetStandingsForGroup_WithValidGroup_ReturnsStandings()
    {
        CreateValidFixture();
        var standings = _service.GetStandingsForGroup('A');
        Assert.IsNotNull(standings);
        Assert.AreEqual(4, standings.Count);
        foreach (var standing in standings)
        {
            Assert.IsNotNull(standing.TeamName);
            Assert.IsTrue(standing.TeamName.StartsWith("Team_A"));
        }
    }

    [TestMethod]
    public void GetStandingsForGroup_WithInvalidGroup_ThrowsArgumentException()
    {
        CreateValidFixture();
        Assert.ThrowsException<ArgumentException>(() => _service.GetStandingsForGroup('Z'));
    }

    [TestMethod]
    public void GetStandingsForGroup_WhenNoFixture_ThrowsException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.GetStandingsForGroup('A'));
    }

    [TestMethod]
    public void HasFixture_WhenFixtureExistsButIsInvalid_ReturnsFalse()
    {
        _fixtureRepository.AddFixture(new Fixture(new TournamentConfig()));
        Assert.IsFalse(_service.HasFixture());
    }

    [TestMethod]
    public void GetFixture_WhenFixtureExists_ShouldReturnCurrentPhase()
    {
        CreateValidFixture();
        Assert.AreEqual("NotStarted", _service.GetFixture().CurrentPhase);
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateFixture()
    {
        AddRequiredTeams();
        AddRequiredStadiums();
        _service.GenerateFixture();
        Assert.IsTrue(_fixtureRepository.ContainsFixture());
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateTwelveGroups()
    {
        AddRequiredTeams();
        AddRequiredStadiums();
        _service.GenerateFixture();
        Assert.AreEqual(12, _fixtureRepository.GetFixture().Groups.Count);
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateFourTeamsPerGroup()
    {
        AddRequiredTeams();
        AddRequiredStadiums();
        _service.GenerateFixture();
        foreach (var group in _fixtureRepository.GetFixture().Groups)
            Assert.AreEqual(4, group.TeamsInGroup.Count);
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateSixMatchesPerGroup()
    {
        AddRequiredTeams();
        AddRequiredStadiums();
        _service.GenerateFixture();
        foreach (var group in _fixtureRepository.GetFixture().Groups)
            Assert.AreEqual(6, group.MatchesInGroup.Count);
    }

    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldStoreGroupMatchesInMatchRepository()
    {
        AddRequiredTeams();
        AddRequiredStadiums();
        _service.GenerateFixture();
        Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);
    }

    [TestMethod]
    public void GenerateFixture_WhenCalledTwice_ShouldKeepOnlyCurrentFixtureMatches()
    {
        AddRequiredTeams();
        AddRequiredStadiums();
        _service.GenerateFixture();
        _service.GenerateFixture();
        Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);
    }
    
    [TestMethod]
    public void GetFixtureEntity_WhenNoFixtureExists_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.GetFixtureEntity());
    }

    [TestMethod]
    public void GetFixtureEntity_WhenFixtureExists_ReturnsFixture()
    {
        CreateValidFixture();
        
        var result = _service.GetFixtureEntity();
        
        Assert.IsNotNull(result);
        Assert.AreEqual(12, result.Groups.Count);
    }
    
    [TestMethod]
    public void GetFixture_WhenGroupsAreEmpty_ReconstructsMatchesFromRepository()
    {
        var baseFixture = CreateValidFixture();
        
        var emptyGroupFixture = new Fixture(_config);
        emptyGroupFixture.InitializeGroups(); 
        _fixtureRepository.RemoveFixture(baseFixture);
        _fixtureRepository.AddFixture(emptyGroupFixture);
        
        var response = _service.GetFixture();
        
        var groupA = response.Groups.First(g => g.GroupLetter == 'A');
        
        Assert.AreEqual(6, groupA.Matches.Count); 
    }

    
    [TestMethod]
    public void DeleteFixture_WhenCalled_ClearsAllRepositoriesAndFixture()
    {
        var fixture = CreateValidFixture();
        
        Assert.IsTrue(_fixtureRepository.ContainsFixture());
        Assert.IsTrue(_matchRepository.GetAllMatches().Count > 0);
        
        _service.DeleteFixture(fixture);
        
        Assert.IsFalse(_fixtureRepository.ContainsFixture());
        Assert.AreEqual(0, _matchRepository.GetAllMatches().Count);
    }
}