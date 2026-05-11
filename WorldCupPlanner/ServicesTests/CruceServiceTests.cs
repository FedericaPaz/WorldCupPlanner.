using Domain;
using IRepository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class CruceServiceTests
{
    private CrucesRepositoryStub _stubCrucesRepository;
    private FixtureRepositoryStub _stubFixtureRepository;
    private StadiumRepositoryStub _stubStadiumRepository;
    private CruceGeneratorServiceStub _stubCruceGenerator;
    private CruceService _service;

    [TestInitialize]
    public void Setup()
    {
        _stubCrucesRepository = new CrucesRepositoryStub();
        _stubFixtureRepository = new FixtureRepositoryStub();
        _stubStadiumRepository = new StadiumRepositoryStub();
        _stubCruceGenerator = new CruceGeneratorServiceStub();

        _service = new CruceService(_stubCrucesRepository, _stubFixtureRepository, _stubStadiumRepository, _stubCruceGenerator);
    }

    private Fixture CreateFixture()
    {
        TournamentConfig config = new TournamentConfig();
        Fixture fixture = new Fixture(config);
        fixture.InitializeGroups();
        List<Group> grupos = Create12GroupsWithTeams();
        foreach (Group grupo in grupos)
        {
            Group grupoFixture = fixture.GetGroups.First(x => x.GroupLetter == grupo.GroupLetter);
            foreach (Team team in grupo.TeamsInGroup)
                grupoFixture.AddTeamToGroup(team);
            foreach (Match match in grupo.MatchesInGroup)
                grupoFixture.AddMatchToGroup(match);
        }
        return fixture;
    }

    private List<Group> Create12GroupsWithTeams()
    {
        List<Group> grupos = new List<Group>();
        for (char letra = 'A'; letra <= 'L'; letra++)
        {
            Group grupo = new Group(letra);
            for (int i = 0; i < 4; i++)
            {
                Team equipo = new Team($"Equipo{letra}{i + 1}", new Confederation("UEFA"), 300 + i);
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
        List<Team> teams = grupo.TeamsInGroup.ToList();
        Match m1 = CreateMatch(teams[0], teams[1], 2, 0, grupo.GroupLetter);
        Match m2 = CreateMatch(teams[0], teams[2], 2, 0, grupo.GroupLetter);
        Match m3 = CreateMatch(teams[0], teams[3], 2, 0, grupo.GroupLetter);
        Match m4 = CreateMatch(teams[1], teams[2], 1, 0, grupo.GroupLetter);
        Match m5 = CreateMatch(teams[1], teams[3], 1, 0, grupo.GroupLetter);
        Match m6 = CreateMatch(teams[2], teams[3], 1, 0, grupo.GroupLetter);
        grupo.AddMatchToGroup(m1);
        grupo.AddMatchToGroup(m2);
        grupo.AddMatchToGroup(m3);
        grupo.AddMatchToGroup(m4);
        grupo.AddMatchToGroup(m5);
        grupo.AddMatchToGroup(m6);
    }

    private Match CreateMatch(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals, char groupLetter)
    {
        Match match = new Match(DateTime.Now.AddHours(-2),
            new Stadium("Centenario", "Montevideo", "Desc", 50000),
            homeTeam, awayTeam, groupLetter, new ClockStub(DateTime.Now));
        match.SetMatchResult(homeGoals, awayGoals);
        return match;
    }

    private List<Cruce> CrearCrucesDummy(int cantidad, TournamentPhase fase)
    {
        List<Cruce> cruces = new List<Cruce>();
        for (int i = 0; i < cantidad; i++)
        {
            Team local = new Team($"Local{i}", new Confederation("UEFA"), 500);
            Team visitante = new Team($"Visitante{i}", new Confederation("CONMEBOL"), 501);
            Match partido = new Match(DateTime.Now.AddHours(-2),
                new Stadium("Estadio", "Ciudad", "Desc", 50000),
                local, visitante, 'A', new ClockStub(DateTime.Now));
            
            partido.SetMatchResult(2, 0);
            Cruce cruce = new Cruce($"C{i}", partido, fase);
            cruces.Add(cruce);
        }
        return cruces;
    }

    [TestMethod]
    public void GenerateRoundOf32_WhenCalled_GeneratesAndAddCruces()
    {
        Fixture fixture = CreateFixture();
        List<Stadium> estadios = CreateStadiums();
        List<Cruce> crucesGenerados = CrearCrucesDummy(16, TournamentPhase.RoundOf32);
        _stubFixtureRepository.AddFixture(fixture);
        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);
        _stubCruceGenerator.SetGeneratedCruces(crucesGenerados);
        _service.GenerateRoundOf32();
        Assert.IsTrue(_stubCrucesRepository.RemoveCalled);
        Assert.AreEqual(crucesGenerados.Count, _stubCrucesRepository.AddedCruces.Count);
        Assert.AreEqual(12, _stubCruceGenerator.LastGroups.Count);
        Assert.AreEqual(4, _stubCruceGenerator.LastStadiums.Count);
        Assert.AreEqual(123, _stubCruceGenerator.LastSeed);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WhenNoFixtureExists_ThrowsException()
    {
        _stubFixtureRepository.AddFixture(null);
        Assert.ThrowsException<ArgumentNullException>(() => _service.GenerateRoundOf32());
    }
    
    [TestMethod]
    public void GenerateNextRound_WhenCurrentRoundIsRoundOf32_GeneratesRoundOf16()
    {
        List<Cruce> crucesActuales = CrearCrucesDummy(16, TournamentPhase.RoundOf32);
        List<Stadium> estadios = CreateStadiums();
        List<Cruce> nuevosCruces = CrearCrucesDummy(8, TournamentPhase.RoundOf16);
        _stubCrucesRepository.SetCruces(crucesActuales);
        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);
        _stubCruceGenerator.SetGeneratedCruces(nuevosCruces);
        _service.GenerateNextRound();
        Assert.AreEqual(16, _stubCruceGenerator.LastRound.Count);
        Assert.AreEqual(4, _stubCruceGenerator.LastStadiums.Count);
        Assert.AreEqual(123, _stubCruceGenerator.LastSeed);
        Assert.AreEqual(nuevosCruces.Count, _stubCrucesRepository.AddedCruces.Count);
    }
    
    [TestMethod]
    public void GenerateNextRound_UsesOnlyLatestPhase()
    {
        List<Cruce> cruces = new List<Cruce>();
        List<Stadium> estadios = CreateStadiums();
        cruces.AddRange(CrearCrucesDummy(16, TournamentPhase.RoundOf32));
        cruces.AddRange(CrearCrucesDummy(8, TournamentPhase.RoundOf16));
        _stubCrucesRepository.SetCruces(cruces);
        foreach (var e in estadios)
            _stubStadiumRepository.AddStadium(e);
        _stubCruceGenerator.SetGeneratedCruces(CrearCrucesDummy(4, TournamentPhase.QuarterFinals));
        _service.GenerateNextRound();
        Assert.AreEqual(TournamentPhase.RoundOf16, _stubCruceGenerator.LastRound[0].Phase);
    }
    
}