using Domain;
using IRepository;
using Repository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class FixtureServiceTests
{
    private FixtureRepository _fixtureRepository;
    private FixtureService _service;
    private ITeamRepository _teamRepository;
    private IStadiumRepository _stadiumRepository;
    private TournamentConfig _config;
    private Fixture _fixture;
    private FixtureGenerator _fixtureGenerator;
    private IMatchRepository _matchRepository;
    private ICrucesRepository _crucesRepository;
    private static readonly DateTime ClockDate = new DateTime(2026, 1, 1);

    [TestInitialize]
    public void Setup()
    {
        _fixtureRepository = new FixtureRepository();
        _teamRepository = new TeamRepository();
        _stadiumRepository = new StadiumRepository();
        _matchRepository = new MatchRepository();
        _crucesRepository = new CrucesRepository();
        _config = new TournamentConfig();
        var clock = new ClockStub(ClockDate);

        _fixtureGenerator = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, clock);
        _service = new FixtureService(
            _fixtureRepository, _fixtureGenerator, _teamRepository,
            _stadiumRepository, _matchRepository, _crucesRepository,_config);
        _fixture = new Fixture(_config);
        Match.ResetIdCounter();
    }

    private Fixture CreateValidFixture()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        var clock = new ClockStub(ClockDate);
        var matchDate = ClockDate.AddMonths(5);

        var stadium = new Stadium("Estadio", "Ciudad", "", 50000);
        _stadiumRepository.AddStadium(stadium);
        foreach (var group in fixture.Groups)
        {
            for (int i = 1; i <= 4; i++)
            {
                var team = new Team($"Team_{group.GroupLetter}_{i}", new Confederation("UEFA"), 1000);
                _teamRepository.Add(team);
                group.AddTeamToGroup(team);
            }
            var teams = group.TeamsInGroup.ToList();
            for (int i = 0; i < 6; i++)
            {
                var match = new Match(matchDate, stadium, teams[0], teams[1], group.GroupLetter, clock);
                group.AddMatchToGroup(match);
            }
        }
        return fixture;
    }
    
    private void AddRequiredTeams()
    {
        AddTeamsForConfederation("UEFA", 16, 1000);
        AddTeamsForConfederation("CONMEBOL", 7, 1200);
        AddTeamsForConfederation("CONCACAF", 7, 1400);
        AddTeamsForConfederation("CAF", 9, 1600);
        AddTeamsForConfederation("AFC", 8, 1800);
        AddTeamsForConfederation("OFC", 1, 2000);
    }

    private void AddTeamsForConfederation(string confederationName, int amount, int rankingBase)
    {
        for (int i = 1; i <= amount; i++)
        {
            var team = new Team(
                $"{confederationName}_Team_{i}",
                new Confederation(confederationName),
                rankingBase + i
            );

            _teamRepository.Add(team);
        }
    }
    
    private void AddRequiredStadiums()
    {
        for (int i = 1; i <= 12; i++)
        {
            var stadium = new Stadium($"Stadium_{i}", "Montevideo", "", 50000);
            _stadiumRepository.AddStadium(stadium);
        }
    }

    [TestMethod]
    public void HasFixture_WhenNoFixture_ReturnsFalse()
    {
        var result = _service.HasFixture();
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasFixture_WhenFixtureExists_ReturnsTrue()
    {
        var fixture = CreateValidFixture();
        _fixtureRepository.AddFixture(fixture);
        var result = _service.HasFixture();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetFixture_WhenNoFixture_ReturnsEmptyResponse()
    {
        var result = _service.GetFixture();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Groups);
        Assert.AreEqual(0, result.Groups.Count);
        Assert.AreEqual("", result.CurrentPhase);
    }

    [TestMethod]
    public void GetFixture_WhenFixtureExists_ReturnsFixtureResponse()
    {
        var fixture = CreateValidFixture();
        _fixtureRepository.AddFixture(fixture);
        var result = _service.GetFixture();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetFixture_ReturnsCorrectGroupData()
    {
        var fixture = CreateValidFixture();
        _fixtureRepository.AddFixture(fixture);
        var result = _service.GetFixture();
        var groupA = result.Groups.First(g => g.GroupLetter == 'A');
        Assert.IsNotNull(groupA);
        Assert.AreEqual(4, groupA.Teams.Count);
        Assert.AreEqual(6, groupA.Matches.Count);
        var firstTeam = groupA.Teams.First();
        Assert.IsTrue(firstTeam.Name.StartsWith("Team_A"));
    }

    [TestMethod]
    public void GetStandingsForGroup_WithValidGroup_ReturnsStandings()
    {
        var fixture = CreateValidFixture();
        _fixtureRepository.AddFixture(fixture);
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
        var fixture = CreateValidFixture();
        _fixtureRepository.AddFixture(fixture);
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
        var invalidFixture = new Fixture(new TournamentConfig());
        _fixtureRepository.AddFixture(invalidFixture);

        var result = _service.HasFixture();

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void GetFixture_WhenFixtureExists_ShouldReturnCurrentPhase()
    {
        var fixture = CreateValidFixture();
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);

        _fixtureRepository.AddFixture(fixture);

        var result = _service.GetFixture();

        Assert.AreEqual("GroupStage", result.CurrentPhase);
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

        var fixture = _fixtureRepository.GetFixture();

        Assert.AreEqual(12, fixture.Groups.Count);
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateFourTeamsPerGroup()
    {
        AddRequiredTeams();
        AddRequiredStadiums();

        _service.GenerateFixture();

        var fixture = _fixtureRepository.GetFixture();

        foreach (var group in fixture.Groups)
        {
            Assert.AreEqual(4, group.TeamsInGroup.Count);
        }
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldCreateSixMatchesPerGroup()
    {
        AddRequiredTeams();
        AddRequiredStadiums();

        _service.GenerateFixture();

        var fixture = _fixtureRepository.GetFixture();

        foreach (var group in fixture.Groups)
        {
            Assert.AreEqual(6, group.MatchesInGroup.Count);
        }
    }
    
    [TestMethod]
    public void GenerateFixture_WithValidData_ShouldStoreGroupMatchesInMatchRepository()
    {
        AddRequiredTeams();
        AddRequiredStadiums();

        _service.GenerateFixture();

        var matches = _matchRepository.GetAllMatches();

        Assert.AreEqual(72, matches.Count);
    }
    
    [TestMethod]
    public void GenerateFixture_WhenCalledTwice_ShouldKeepOnlyCurrentFixtureMatches()
    {
        AddRequiredTeams();
        AddRequiredStadiums();

        _service.GenerateFixture();
        _service.GenerateFixture();

        var matches = _matchRepository.GetAllMatches();

        Assert.AreEqual(72, matches.Count);
    }
}