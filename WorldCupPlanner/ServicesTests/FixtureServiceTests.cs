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
    private FixtureRepository _fixtureRepository = null!;
    private FixtureService _service = null!;
    private ITeamRepository _teamRepository = null!;
    private IStadiumRepository _stadiumRepository = null!;
    private TournamentConfig _config = null!;
    private Fixture _fixture = null!;
    private FixtureGenerator _fixtureGenerator = null!;
    private IMatchRepository _matchRepository = null!;
    private ICrucesRepository _crucesRepository = null!;
    private static readonly DateTime ClockDate = new DateTime(2026, 1, 1);

    [TestInitialize]
    public void Setup()
    {
        _fixtureRepository = new FixtureRepository();
        _teamRepository    = new TeamRepository();
        _stadiumRepository = new StadiumRepositoryStub();
        _matchRepository   = new MatchRepository();
        _crucesRepository  = new CrucesRepository();
        _config            = new TournamentConfig();
        var clock          = new ClockStub(ClockDate);

        _fixtureGenerator = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, clock);
        _service = new FixtureService(
            _fixtureRepository, _fixtureGenerator, _teamRepository,
            _stadiumRepository, _matchRepository, _crucesRepository, _config);
        _fixture = new Fixture(_config);
    }

    private Fixture CreateValidFixture()
    {
        var config    = new TournamentConfig();
        var fixture   = new Fixture(config);
        fixture.InitializeGroups();
        var clock     = new ClockStub(ClockDate);
        var matchDate = ClockDate.AddMonths(5);
        var stadium   = new Stadium("Estadio", "Ciudad", "", 50000);
        int matchId   = 1;

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
                var match = new Match(matchId++, matchDate, stadium, teams[0], teams[1], group.GroupLetter, clock);
                group.AddMatchToGroup(match);
            }
        }
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
        _fixtureRepository.AddFixture(CreateValidFixture());
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
        _fixtureRepository.AddFixture(CreateValidFixture());
        Assert.IsNotNull(_service.GetFixture());
    }

    [TestMethod]
    public void GetFixture_ReturnsCorrectGroupData()
    {
        _fixtureRepository.AddFixture(CreateValidFixture());
        var groupA = _service.GetFixture().Groups.First(g => g.GroupLetter == 'A');
        Assert.IsNotNull(groupA);
        Assert.AreEqual(4, groupA.Teams.Count);
        Assert.AreEqual(6, groupA.Matches.Count);
        Assert.IsTrue(groupA.Teams.First().Name.StartsWith("Team_A"));
    }

    [TestMethod]
    public void GetStandingsForGroup_WithValidGroup_ReturnsStandings()
    {
        _fixtureRepository.AddFixture(CreateValidFixture());
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
        _fixtureRepository.AddFixture(CreateValidFixture());
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
        var fixture = CreateValidFixture();
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        _fixtureRepository.AddFixture(fixture);
        Assert.AreEqual("GroupStage", _service.GetFixture().CurrentPhase);
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
}