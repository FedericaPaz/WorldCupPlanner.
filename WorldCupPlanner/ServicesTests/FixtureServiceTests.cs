using Domain;
using DTOs;
using IRepository;
using Repository;
using Services;
using Services.Mappers;

namespace ServicesTests;

[TestClass]
public class FixtureServiceTests
{
    private FixtureRepository _fixtureRepository;
    private FixtureService _service;
    private TournamentConfig _config;
    private Fixture _fixture;
    private FixtureGenerator _fixtureGenerator;
    
    [TestInitialize]
    public void Setup()
    {
        _fixtureRepository = new FixtureRepository();
        _service = new FixtureService(_fixtureRepository, _fixtureGenerator);
        _config = new TournamentConfig();
        _fixture = new Fixture(_config);
        Match.ResetIdCounter();
    }
    
    private Fixture CreateValidFixture()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
    
        foreach (var group in fixture.GetGroups)
        {
            for (int i = 1; i <= 4; i++)
            {
                var team = new Team($"Team_{group.GroupLetter}_{i}", new Confederation("UEFA"), 1000);
                group.AddTeamToGroup(team);
            }
        
            var clock = new ClockStub(DateTime.Now);
            var stadium = new Stadium("Estadio", "Ciudad", "", 50000);
            var teams = group.TeamsInGroup.ToList();
        
            for (int i = 0; i < 6; i++)
            {
                var match = new Match(clock.Now, stadium, teams[0], teams[1], group.GroupLetter, clock);
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