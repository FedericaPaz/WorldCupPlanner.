using Domain.Entities;
using DomainTests.Stubs;
using IRepository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class FixtureServiceTests
{
    private FixtureService _service = null!;
    private FixtureRepositoryStub _fixtureRepository = null!;
    private TeamRepositoryStub _teamRepository = null!;
    private StadiumRepositoryStub _stadiumRepository = null!;
    private IMatchRepository _matchRepository = null!;
    private CrucesRepositoryStub _crucesRepository = null!;
    private TournamentConfig _config = null!;
    private DateTimeMatchStub _clock = null!;

    [TestInitialize]
    public void Setup()
    {
        Fixture.RegisterEngine("Aleatorio puro", () => new PureRandomSimulationEngine());
        Fixture.RegisterEngine("Probabilístico", () => new ProbabilisticSimulationEngine());

        _fixtureRepository = new FixtureRepositoryStub();
        _teamRepository = new TeamRepositoryStub();
        _stadiumRepository = new StadiumRepositoryStub();
        _matchRepository = new MatchRepositoryStub();
        _crucesRepository = new CrucesRepositoryStub();
        _config = new TournamentConfig();
        _clock = new DateTimeMatchStub(new DateTime(2026, 1, 1));

        _service = new FixtureService(
            _fixtureRepository,
            _teamRepository,
            _stadiumRepository,
            _matchRepository,
            _crucesRepository,
            _config,
            _clock);
    }

    private void Add48Teams()
    {
        var confederations = new (string name, int count)[]
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7),
            ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };

        int ranking = 1000;
        foreach (var (confName, count) in confederations)
            for (int i = 0; i < count; i++)
                _teamRepository.Add(new Team($"{confName}_{i + 1:D2}", new Confederation(confName), ranking++));
    }

    private void Add4Stadiums()
    {
        for (int i = 1; i <= 4; i++)
            _stadiumRepository.AddStadium(new Stadium($"Estadio{i}", "Ciudad", null, 50000));
    }

    [TestMethod]
    public void HasFixture_WhenNoFixtureExists_ReturnsFalse()
    {
        Assert.IsFalse(_service.HasFixture());
    }

    [TestMethod]
    public void HasFixture_WhenFixtureExists_ReturnsTrue()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        Assert.IsTrue(_service.HasFixture());
    }

    [TestMethod]
    public void GetFixture_WhenNoFixture_ReturnsEmptyResponse()
    {
        var result = _service.GetFixture();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Groups.Count);
        Assert.AreEqual(string.Empty, result.CurrentPhase);
    }

    [TestMethod]
    public void GenerateFixture_WithLessThan48Teams_ThrowsInvalidOperationException()
    {
        Add4Stadiums();
        _teamRepository.Add(new Team("Uruguay", new Confederation("CONMEBOL"), 1000));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GenerateFixture("Probabilístico"));
    }

    [TestMethod]
    public void GenerateFixture_WithNoStadiums_ThrowsInvalidOperationException()
    {
        Add48Teams();

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GenerateFixture("Probabilístico"));
    }

    [TestMethod]
    public void GenerateFixture_With48TeamsAnd4Stadiums_StoresFixture()
    {
        Add48Teams();
        Add4Stadiums();

        _service.GenerateFixture("Probabilístico");

        Assert.IsTrue(_fixtureRepository.ContainsFixture());
    }

    [TestMethod]
    public void GenerateFixture_With48TeamsAnd4Stadiums_Creates72Matches()
    {
        Add48Teams();
        Add4Stadiums();

        _service.GenerateFixture("Probabilístico");

        Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);
    }

    [TestMethod]
    public void GenerateFixture_WhenCalledTwice_ReplacesExistingFixture()
    {
        Add48Teams();
        Add4Stadiums();

        _service.GenerateFixture("Probabilístico");
        _service.GenerateFixture("Probabilístico");

        Assert.IsTrue(_fixtureRepository.ContainsFixture());
        Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);
    }

    [TestMethod]
    public void GenerateFixture_ResetsTeamRankingsToFifa()
    {
        Add48Teams();
        Add4Stadiums();

        _service.GenerateFixture("Probabilístico");

        foreach (var team in _teamRepository.GetAll())
            Assert.AreEqual(team.FifaRanking, team.CurrentRanking);
    }

    [TestMethod]
    public void GenerateFixture_Creates12Groups()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        var result = _service.GetFixture();

        Assert.AreEqual(12, result.Groups.Count);
    }

    [TestMethod]
    public void GenerateFixture_EachGroupHas4Teams()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        var result = _service.GetFixture();

        foreach (var group in result.Groups)
            Assert.AreEqual(4, group.Teams.Count);
    }

    [TestMethod]
    public void GenerateFixture_NoGroupHasMoreThan2TeamsFromSameConfederation()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        var result = _service.GetFixture();

        foreach (var group in result.Groups)
        {
            var maxPerConf = group.Teams
                .GroupBy(t => t.ConfederationName)
                .Max(g => g.Count());

            Assert.IsTrue(maxPerConf <= 2,
                $"Grupo {group.GroupLetter} tiene más de 2 equipos de la misma confederación.");
        }
    }

    [TestMethod]
    public void DeleteFixture_RemovesFixtureFromRepository()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        _service.DeleteFixture();

        Assert.IsFalse(_fixtureRepository.ContainsFixture());
    }

    [TestMethod]
    public void DeleteFixture_ClearsAllMatches()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        _service.DeleteFixture();

        Assert.AreEqual(0, _matchRepository.GetAllMatches().Count);
    }

    [TestMethod]
    public void DeleteFixture_ClearsCruces()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        _service.DeleteFixture();

        Assert.IsTrue(_crucesRepository.RemoveCalled);
    }

    [TestMethod]
    public void DeleteFixture_WhenNoFixture_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.DeleteFixture());
    }

    [TestMethod]
    public void DeleteFixture_ResetsTeamRankingsToFifa()
    {
        Add48Teams();
        Add4Stadiums();
        _service.GenerateFixture("Probabilístico");

        _service.DeleteFixture();

        foreach (var team in _teamRepository.GetAll())
            Assert.AreEqual(team.FifaRanking, team.CurrentRanking);
    }
    
    [TestMethod]
public void GetFixtureEntity_WhenNoFixture_ThrowsInvalidOperationException()
{
    Assert.ThrowsException<InvalidOperationException>(() =>
        _service.GetFixtureEntity());
}

[TestMethod]
public void GetFixtureEntity_WhenFixtureExists_ReturnsFixture()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    var fixture = _service.GetFixtureEntity();

    Assert.IsNotNull(fixture);
    Assert.AreEqual(12, fixture.Groups.Count);
}

[TestMethod]
public void GetFixtureEntity_WhenFixtureExists_LoadsMatchesIntoGroups()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    var fixture = _service.GetFixtureEntity();

    var totalMatchesInGroups = fixture.Groups.Sum(g => g.MatchesInGroup.Count);

    Assert.AreEqual(72, totalMatchesInGroups);
}

[TestMethod]
public void GetStandingsForGroup_WhenFixtureExists_ReturnsStandingsForGroup()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    var standings = _service.GetStandingsForGroup('A');

    Assert.IsNotNull(standings);
    Assert.AreEqual(4, standings.Count);
}

[TestMethod]
public void GetStandingsForGroup_WhenGroupDoesNotExist_ThrowsArgumentException()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    Assert.ThrowsException<ArgumentException>(() =>
        _service.GetStandingsForGroup('Z'));
}

[TestMethod]
public void GenerateFixture_WithDefaultEngineName_ShouldStoreFixture()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture();

    Assert.IsTrue(_fixtureRepository.ContainsFixture());
    Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);
}

[TestMethod]
public void GenerateFixture_WhenCalled_ShouldRemovePreviousCruces()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    Assert.IsTrue(_crucesRepository.RemoveCalled);
}

[TestMethod]
public void GenerateFixture_WhenCalled_ShouldClearPreviousMatches()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);

    _service.GenerateFixture("Probabilístico");

    Assert.AreEqual(72, _matchRepository.GetAllMatches().Count);
}

[TestMethod]
public void HasFixture_WhenExistingFixtureIsNoLongerValid_ShouldReturnFalseAndRemoveFixture()
{
    Add48Teams();
    Add4Stadiums();

    _service.GenerateFixture("Probabilístico");

    _teamRepository.Clear();

    var result = _service.HasFixture();

    Assert.IsFalse(result);
    Assert.IsFalse(_fixtureRepository.ContainsFixture());
}
}