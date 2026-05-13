using Domain;
using IRepository;
using Repository;
using Services;

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
            _stadiumRepository, _matchRepository, _crucesRepository);
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
        foreach (var group in fixture.GetGroups)
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
}