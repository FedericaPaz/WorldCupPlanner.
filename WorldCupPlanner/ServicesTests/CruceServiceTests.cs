using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DomainTests.Stubs;
using IRepository;
using Services;
using Services.Simulation;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class CruceServiceTests
{
    private CruceService _service = null!;
    private CrucesRepositoryStub _crucesRepository = null!;
    private StadiumRepositoryStub _stadiumRepository = null!;
    private IMatchRepository _matchRepository = null!;
    private FixtureRepositoryStub _fixtureRepository = null!;
    private TeamRepositoryStub _teamRepository = null!;
    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private TournamentConfig _config = null!;
    private DateTimeMatchStub _clock = null!;
    private ISimulationEngineFactory _engineFactory = null!;

    [TestInitialize]
    public void Setup()
    {
        Fixture.RegisterEngine("Aleatorio puro", () => new PureRandomSimulationEngine());
        Fixture.RegisterEngine("Probabilístico", () => new ProbabilisticSimulationEngine());

        _crucesRepository = new CrucesRepositoryStub();
        _stadiumRepository = new StadiumRepositoryStub();
        _matchRepository = new MatchRepositoryStub();
        _fixtureRepository = new FixtureRepositoryStub();
        _teamRepository = new TeamRepositoryStub();
        _logRepository = new LogRepositoryStub();
        _logService = new LogService(_logRepository);
        _config = new TournamentConfig();
        _clock = new DateTimeMatchStub(new DateTime(2026, 1, 1));
        _engineFactory = new DefaultSimulationEngineFactory(new List<ISimulationEngine>
        {
            new PureRandomSimulationEngine(),
            new ProbabilisticSimulationEngine()
        });

        _service = new CruceService(
            _crucesRepository,
            _stadiumRepository,
            _config,
            _matchRepository,
            _fixtureRepository,
            _clock,
            _engineFactory,
            _teamRepository,
            _logService);
    }

    private Team CreateTeam(string name, string confederation = "CONMEBOL", int ranking = 1000)
    {
        var team = new Team(name, new Confederation(confederation), ranking);
        _teamRepository.Add(team);
        return team;
    }

    private Stadium CreateStadium(string name = "Estadio1")
    {
        var stadium = new Stadium(name, "Ciudad", null, 50000);
        _stadiumRepository.AddStadium(stadium);
        return stadium;
    }

    private Fixture CreateFixtureWithEngine()
    {
        var fixture = new Fixture(_config);
        fixture.SetSimulationEngine("Probabilístico");
        _fixtureRepository.AddFixture(fixture);
        return fixture;
    }

    private Match CreateMatchWithResult(Team home, Team away, Stadium stadium, int homeGoals, int awayGoals)
    {
        var match = new Match(
            _matchRepository.GetNextId(),
            new DateTime(2027, 6, 10),
            stadium,
            home, away, 'N', _clock);
        match.SimulateResult(homeGoals, awayGoals);
        _matchRepository.AddMatch(match);
        return match;
    }

    [TestMethod]
    public void GetAllCruces_WhenNoCruces_ReturnsEmptyList()
    {
        var result = _service.GetAllCruces();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_WhenNoFixture_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateRoundOf32());
    }

    [TestMethod]
    public void GenerateNextRound_WhenNoCruces_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void GenerateNextRound_WhenCrucesHaveNoResult_ThrowsInvalidOperationException()
    {
        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");
        var stadium = CreateStadium();

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);

        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf32);
        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void SimulateCruces_WhenNoFixture_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.SimulateCruces());
    }

    [TestMethod]
    public void SimulateCruces_WithPendingCruces_SetsResultOnAllMatches()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);

        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf32);
        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        _service.SimulateCruces();

        Assert.IsTrue(match.HomeGoals.HasValue);
        Assert.IsTrue(match.AwayGoals.HasValue);
    }

    [TestMethod]
    public void SimulateCruces_NeverProducesDrawResult()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);

        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        Assert.AreNotEqual(match.HomeGoals, match.AwayGoals);
    }

    [TestMethod]
    public void SimulateCruces_UpdatesTeamRankingsAfterMatch()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay", "CONMEBOL", 1500);
        var away = CreateTeam("Argentina", "UEFA", 1200);

        int rankingHomeAntes = home.CurrentRanking;
        int rankingAwayAntes = away.CurrentRanking;

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);
        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        var homeActualizado = _teamRepository.Get("Uruguay")!;
        var awayActualizado = _teamRepository.Get("Argentina")!;

        Assert.AreNotEqual(rankingHomeAntes, homeActualizado.CurrentRanking,
            "El ranking del equipo local debería haber cambiado.");
        Assert.AreNotEqual(rankingAwayAntes, awayActualizado.CurrentRanking,
            "El ranking del equipo visitante debería haber cambiado.");
    }

    [TestMethod]
    public void SimulateCruces_LogsRankingUpdate()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = new Match(1, new DateTime(2027, 6, 10), stadium, home, away, 'N', _clock);
        _matchRepository.AddMatch(match);
        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        var logs = _logRepository.GetAll();
        Assert.IsTrue(logs.Any(l => l.Category == "Ranking"),
            "Se debería haber registrado un log de ranking.");
    }

    [TestMethod]
    public void SimulateCruces_SkipsCrucesWithExistingResult()
    {
        CreateFixtureWithEngine();
        var stadium = CreateStadium();

        var home = CreateTeam("Uruguay");
        var away = CreateTeam("Argentina", "UEFA");

        var match = CreateMatchWithResult(home, away, stadium, 2, 1);
        _crucesRepository.SetCruces(new List<Cruce> { new Cruce("A1", match, TournamentPhase.RoundOf32) });

        _service.SimulateCruces();

        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }
}