using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using IRepository;
using Services;
using ServicesTests.Stubs;
using Services.Simulation;

namespace ServicesTests;

[TestClass]
public class CruceServiceTests
{
    private static readonly DateTime FechaCreacionClock = new DateTime(2026, 1, 1);
    private static readonly DateTime FechaPartido = new DateTime(2026, 6, 25);
    private static readonly DateTime FechaResultadoClock = new DateTime(2026, 6, 26);

    private CrucesRepositoryStub _stubCrucesRepository = null!;
    private FixtureRepositoryStub _stubFixtureRepository = null!;
    private StadiumRepositoryStub _stubStadiumRepository = null!;
    private CruceService _service = null!;
    private IMatchRepository _matchRepository = null!;
    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private RankingServiceStub _rankingService = null!;
    private IDateTimeMatch _clock = null!;
    private TournamentConfig _config = null!;
    private NotificationServiceStub _notificationServiceStub = null!;
    private int _nextId;

    [TestInitialize]
    public void Setup()
    {
        _nextId = 1;
        _stubCrucesRepository = new CrucesRepositoryStub();
        _stubFixtureRepository = new FixtureRepositoryStub();
        _stubStadiumRepository = new StadiumRepositoryStub();
        _matchRepository = new MatchRepositoryStub();
        _logRepository = new LogRepositoryStub();
        _logService = new LogService(_logRepository);
        _rankingService = new RankingServiceStub();
        _clock = new ClockStub(FechaCreacionClock);
        _config = new TournamentConfig();
        _notificationServiceStub = new NotificationServiceStub();

        var engineFactory = new DefaultSimulationEngineFactory();

        _service = new CruceService(
            _stubCrucesRepository,
            _stubStadiumRepository,
            _config,
            _matchRepository,
            _stubFixtureRepository,
            _clock,
            engineFactory);
    }

    private Fixture CreateFixtureWithResults()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();

        var grupos = Create12GroupsWithTeams();

        foreach (var grupo in grupos)
        {
            var grupoFixture = fixture.Groups.First(x => x.GroupLetter == grupo.GroupLetter);

            foreach (var team in grupo.TeamsInGroup)
                grupoFixture.AddTeamToGroup(team);

            foreach (var match in grupo.MatchesInGroup)
                grupoFixture.AddMatchToGroup(match);
        }

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);

        return fixture;
    }

    private List<Group> Create12GroupsWithTeams()
    {
        var grupos = new List<Group>();

        for (char letra = 'A'; letra <= 'L'; letra++)
        {
            var grupo = new Group(letra);

            for (int i = 0; i < 4; i++)
            {
                var equipo = new Team($"Equipo{letra}{i + 1}", new Confederation("UEFA"), 300 + i * 10);
                grupo.AddTeamToGroup(equipo);
            }

            CreateResults(grupo);
            grupos.Add(grupo);
        }

        return grupos;
    }

    private List<Stadium> CreateStadiums()
    {
        return new List<Stadium>
        {
            new Stadium("Estadio 1", "Ciudad 1", "Desc", 50000),
            new Stadium("Estadio 2", "Ciudad 2", "Desc", 60000),
            new Stadium("Estadio 3", "Ciudad 3", "Desc", 70000),
            new Stadium("Estadio 4", "Ciudad 4", "Desc", 80000)
        };
    }

    private void CreateResults(Group grupo)
    {
        var teams = grupo.TeamsInGroup.ToList();

        grupo.AddMatchToGroup(CreateMatch(teams[0], teams[1], 2, 0, grupo.GroupLetter));
        grupo.AddMatchToGroup(CreateMatch(teams[0], teams[2], 2, 0, grupo.GroupLetter));
        grupo.AddMatchToGroup(CreateMatch(teams[0], teams[3], 2, 0, grupo.GroupLetter));
        grupo.AddMatchToGroup(CreateMatch(teams[1], teams[2], 1, 0, grupo.GroupLetter));
        grupo.AddMatchToGroup(CreateMatch(teams[1], teams[3], 1, 0, grupo.GroupLetter));
        grupo.AddMatchToGroup(CreateMatch(teams[2], teams[3], 1, 0, grupo.GroupLetter));
    }

    private Match CreateMatch(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals, char groupLetter)
    {
        var match = new Match(
            _nextId++,
            FechaPartido,
            new Stadium("Centenario", "Montevideo", "Desc", 50000),
            homeTeam,
            awayTeam,
            groupLetter,
            new ClockStub(FechaCreacionClock));

        match.SetClock(new ClockStub(FechaResultadoClock));
        match.SetMatchResult(homeGoals, awayGoals);

        return match;
    }

    private List<Cruce> CrearCrucesDummy(int cantidad, TournamentPhase fase)
    {
        var cruces = new List<Cruce>();

        for (int i = 0; i < cantidad; i++)
        {
            var local = new Team($"Local{i}", new Confederation("UEFA"), 500);
            var visitante = new Team($"Visitante{i}", new Confederation("CONMEBOL"), 501);

            var partido = new Match(
                _nextId++,
                FechaPartido,
                new Stadium("Estadio", "Ciudad", "Desc", 50000),
                local,
                visitante,
                'N',
                new ClockStub(FechaCreacionClock));

            partido.SetClock(new ClockStub(FechaResultadoClock));
            partido.SetMatchResult(2, 0);

            cruces.Add(new Cruce($"C{i}", partido, fase));
        }

        return cruces;
    }

    [TestMethod]
    public void GenerateRoundOf32_WhenCalled_GeneratesAndAddsCruces()
    {
        var fixture = CreateFixtureWithResults();
        var estadios = CreateStadiums();

        _stubFixtureRepository.AddFixture(fixture);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateRoundOf32();

        Assert.IsTrue(_stubCrucesRepository.RemoveCalled);
        Assert.AreEqual(16, _stubCrucesRepository.AddedCruces.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_WhenNoFixtureExists_ThrowsException()
    {
        _stubFixtureRepository.AddFixture(null);

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateRoundOf32());
    }

    [TestMethod]
    public void GenerateNextRound_WhenCurrentRoundIsRoundOf32_GeneratesRoundOf16()
    {
        var crucesActuales = CrearCrucesDummy(16, TournamentPhase.RoundOf32);
        var estadios = CreateStadiums();

        _stubCrucesRepository.SetCruces(crucesActuales);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateNextRound();

        Assert.IsTrue(_stubCrucesRepository.AddedCruces.Count > 0);
    }

    [TestMethod]
    public void GenerateNextRound_UsesOnlyLatestPhase()
    {
        var cruces = new List<Cruce>();

        cruces.AddRange(CrearCrucesDummy(16, TournamentPhase.RoundOf32));
        cruces.AddRange(CrearCrucesDummy(8, TournamentPhase.RoundOf16));

        var estadios = CreateStadiums();

        _stubCrucesRepository.SetCruces(cruces);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateNextRound();

        Assert.IsTrue(_stubCrucesRepository.AddedCruces.All(c => c.Phase == TournamentPhase.QuarterFinals));
    }

    [TestMethod]
    public void GenerateNextRound_WhenNoCrucesExist_ThrowsException()
    {
        _stubCrucesRepository.SetCruces(new List<Cruce>());

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void GenerateNextRound_WhenCurrentPhaseIsFinal_ThrowsException()
    {
        var crucesFinal = CrearCrucesDummy(1, TournamentPhase.Final);

        _stubCrucesRepository.SetCruces(crucesFinal);

        Assert.ThrowsException<InvalidOperationException>(() => _service.GenerateNextRound());
    }

    [TestMethod]
    public void GetAllCruces_WhenCrucesExist_ReturnsMappedResponses()
    {
        var cruces = CrearCrucesDummy(16, TournamentPhase.RoundOf32);

        _stubCrucesRepository.SetCruces(cruces);

        var result = _service.GetAllCruces();

        Assert.AreEqual(16, result.Count);
    }

    [TestMethod]
    public void GetAllCruces_WhenNoCruces_ReturnsEmptyList()
    {
        _stubCrucesRepository.SetCruces(new List<Cruce>());

        var result = _service.GetAllCruces();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void SimulateCruces_WithSpecificEngine_ShouldCallSimulationServiceWithGivenEngine()
    {
        var fixture = CreateFixtureWithResults();
        var estadios = CreateStadiums();

        _stubFixtureRepository.AddFixture(fixture);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateRoundOf32();

        Assert.IsNotNull(_stubCrucesRepository.AddedCruces);
    }

    [TestMethod]
    public void GenerateRoundOf32_NoDuplicateTeams()
    {
        var fixture = CreateFixtureWithResults();
        var estadios = CreateStadiums();

        _stubFixtureRepository.AddFixture(fixture);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateRoundOf32();

        var cruces = _stubCrucesRepository.AddedCruces;
        var todosEquipos = new HashSet<string>();

        foreach (var cruce in cruces)
        {
            Assert.IsFalse(
                todosEquipos.Contains(cruce.Match.HomeTeam.Name),
                $"Equipo {cruce.Match.HomeTeam.Name} aparece múltiples veces");

            Assert.IsFalse(
                todosEquipos.Contains(cruce.Match.AwayTeam.Name),
                $"Equipo {cruce.Match.AwayTeam.Name} aparece múltiples veces");

            todosEquipos.Add(cruce.Match.HomeTeam.Name);
            todosEquipos.Add(cruce.Match.AwayTeam.Name);
        }

        Assert.AreEqual(32, todosEquipos.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldHaveCorrectStructure_A8_B8()
    {
        var fixture = CreateFixtureWithResults();
        var estadios = CreateStadiums();

        _stubFixtureRepository.AddFixture(fixture);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateRoundOf32();

        var cruces = _stubCrucesRepository.AddedCruces;

        Assert.AreEqual(8, cruces.Count(c => c.Name.StartsWith("A")), "8 cruces tipo A");
        Assert.AreEqual(8, cruces.Count(c => c.Name.StartsWith("B")), "8 cruces tipo B");
    }

    [TestMethod]
    public void GenerateRoundOf32_AllCrucesHaveValidMatch()
    {
        var fixture = CreateFixtureWithResults();
        var estadios = CreateStadiums();

        _stubFixtureRepository.AddFixture(fixture);

        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);

        _service.GenerateRoundOf32();

        foreach (var cruce in _stubCrucesRepository.AddedCruces)
        {
            Assert.IsNotNull(cruce.Match);
            Assert.IsNotNull(cruce.Match.HomeTeam);
            Assert.IsNotNull(cruce.Match.AwayTeam);
            Assert.IsNotNull(cruce.Match.Stadium);
            Assert.AreNotEqual(cruce.Match.HomeTeam.Name, cruce.Match.AwayTeam.Name);
        }
    }
}