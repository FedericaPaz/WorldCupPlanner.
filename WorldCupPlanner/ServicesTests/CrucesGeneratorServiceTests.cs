using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class CrucesGeneratorServiceTests
{
    private static readonly TournamentConfig ConfigTest = new TournamentConfig
    {
        FechaInicioTorneo = new DateTime(2027, 1, 1)
    };

    private static readonly DateTime FechaCreacionClock  = new DateTime(2027, 6, 10);
    private static readonly DateTime FechaPartido        = new DateTime(2027, 6, 12);
    private static readonly DateTime FechaResultadoClock = new DateTime(2027, 6, 15);

    private CrucesGeneratorService _service;
    private List<Stadium> _estadios;

    [TestInitialize]
    public void TestInitialize()
    {
        _service  = new CrucesGeneratorService(ConfigTest);
        _estadios = CreateStadiums();
    }

    private Fixture CreateFixtureWithGroups(List<Group> grupos)
    {
        var fixture = new Fixture(ConfigTest);
        fixture.SetGroups(grupos);
        return fixture;
    }

    private Fixture CreateDefaultFixture() =>
        CreateFixtureWithGroups(Create12GroupsWithTeams());

    private List<Group> Create12GroupsWithTeams()
    {
        var grupos = new List<Group>();
        for (char letra = 'A'; letra <= 'L'; letra++)
        {
            var grupo = new Group(letra);
            for (int i = 0; i < 4; i++)
                grupo.AddTeamToGroup(new Team($"Equipo{letra}{i + 1}", new Confederation("UEFA"), 300 + i));
            CreateResults(grupo);
            grupos.Add(grupo);
        }
        return grupos;
    }

    private List<Stadium> CreateStadiums() => new List<Stadium>
    {
        new Stadium("Estadio 1", "Ciudad 1", "Descripción 1", 50000),
        new Stadium("Estadio 2", "Ciudad 2", "Descripción 2", 60000),
        new Stadium("Estadio 3", "Ciudad 3", "Descripción 3", 70000),
        new Stadium("Estadio 4", "Ciudad 4", "Descripción 4", 80000)
    };

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
        var match = new Match(FechaPartido,
            new Stadium("Centenario", "Montevideo", "Desc", 50000),
            homeTeam, awayTeam, groupLetter,
            new ClockStub(FechaCreacionClock));
        match.SetClock(new ClockStub(FechaResultadoClock));
        match.SetMatchResult(homeGoals, awayGoals);
        return match;
    }

    private void SimulateWinner(List<Cruce> cruces)
    {
        foreach (var cruce in cruces)
            cruce.Partido.SimulateResult(2, 0);
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldGenerate16Cruces()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, ConfigTest.SemillaFixture);
        Assert.AreEqual(16, cruces.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_WithEmptyGroups_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.GenerateRoundOf32(CreateFixtureWithGroups(new List<Group>()), _estadios, ConfigTest.SemillaFixture));
    }

    [TestMethod]
    public void GenerateRoundOf32_WithNullGroups_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.GenerateRoundOf32(CreateFixtureWithGroups(null), _estadios, ConfigTest.SemillaFixture));
    }

    [TestMethod]
    public void GenerateRoundOf32_WithEmptyStadiums_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _service.GenerateRoundOf32(CreateDefaultFixture(), new List<Stadium>(), ConfigTest.SemillaFixture));
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldUseQualifiedTeams()
    {
        var grupos = Create12GroupsWithTeams();
        var cruces = _service.GenerateRoundOf32(CreateFixtureWithGroups(grupos), _estadios, ConfigTest.SemillaFixture);

        bool encontrado = grupos
            .SelectMany(g => g.TeamsInGroup)
            .Any(e => e.Name == cruces[0].Partido.HomeTeam.Name);

        Assert.IsTrue(encontrado);
    }

    [TestMethod]
    public void GenerateRoundOf32_WithLessThan12Groups_ShouldThrowException()
    {
        var grupos = Create12GroupsWithTeams().Take(6).ToList();
        Assert.ThrowsException<ArgumentException>(() =>
            _service.GenerateRoundOf32(CreateFixtureWithGroups(grupos), _estadios, ConfigTest.SemillaFixture));
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldGenerateRoundOf32Phase()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, ConfigTest.SemillaFixture);
        Assert.AreEqual(TournamentPhase.RoundOf32, cruces[0].Phase);
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldRotateStadiums()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, ConfigTest.SemillaFixture);
        Assert.AreEqual("Estadio 1", cruces[0].Partido.Stadium.StadiumName);
        Assert.AreEqual("Estadio 2", cruces[1].Partido.Stadium.StadiumName);
    }

    [TestMethod]
    public void GenerateRoundOf32_WithSameSeed_ShouldGenerateSameOrder()
    {
        int semilla = ConfigTest.SemillaFixture;
        var cruces1 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, semilla);
        var cruces2 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, semilla);
 
        for (int i = 0; i < cruces1.Count; i++)
        {
            Assert.AreEqual(cruces1[i].Partido.HomeTeam.Name, cruces2[i].Partido.HomeTeam.Name, $"Cruce {i}: local difiere con la misma semilla");
            Assert.AreEqual(cruces1[i].Partido.AwayTeam.Name, cruces2[i].Partido.AwayTeam.Name, $"Cruce {i}: visitante difiere con la misma semilla");
        }
    }

    [TestMethod]
    public void GenerateRoundOf32_WithDifferentSeed_ShouldGenerateDifferentOrder()
    {
        var cruces1 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 100);
        var cruces2 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 99999);
 
        bool isDifferent = false;
        for (int i = 0; i < cruces1.Count; i++)
        {
            if (cruces1[i].Partido.HomeTeam.Name != cruces2[i].Partido.HomeTeam.Name || cruces1[i].Partido.AwayTeam.Name != cruces2[i].Partido.AwayTeam.Name)
            {
                isDifferent = true;
                break;
            }
        }
        Assert.IsTrue(isDifferent, "Con semillas muy distintas debería haber al menos un emparejamiento diferente");
    }

    [TestMethod]
    public void GenerateNextRound_ShouldGenerateRoundOf16()
    {
        var cruces32 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        SimulateWinner(cruces32);
        var cruces16 = _service.GenerateNextRound(cruces32, _estadios, 123);
        Assert.AreEqual(TournamentPhase.RoundOf16, cruces16[0].Phase);
        Assert.AreEqual(8, cruces16.Count);
    }

    [TestMethod]
    public void GenerateNextRound_WithoutWinners_ShouldThrowException()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.GenerateNextRound(cruces, _estadios, 123));
    }

    [TestMethod]
    public void GenerateNextRound_FromRoundOf16_ShouldGenerateQuarterFinals()
    {
        var cruces32 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        SimulateWinner(cruces32);
        var cruces16 = _service.GenerateNextRound(cruces32, _estadios, 123);
        SimulateWinner(cruces16);
        var cuartos = _service.GenerateNextRound(cruces16, _estadios, 123);
        Assert.AreEqual(TournamentPhase.QuarterFinals, cuartos[0].Phase);
        Assert.AreEqual(4, cuartos.Count);
    }

    [TestMethod]
    public void GenerateNextRound_FromQuarters_ShouldGenerateSemiFinals()
    {
        var cruces32 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        SimulateWinner(cruces32);
        var cruces16 = _service.GenerateNextRound(cruces32, _estadios, 123);
        SimulateWinner(cruces16);
        var cuartos = _service.GenerateNextRound(cruces16, _estadios, 123);
        SimulateWinner(cuartos);
        var semis = _service.GenerateNextRound(cuartos, _estadios, 123);
        Assert.AreEqual(TournamentPhase.SemiFinals, semis[0].Phase);
        Assert.AreEqual(2, semis.Count);
    }

    [TestMethod]
    public void GenerateNextRound_FromSemis_ShouldGenerateFinals()
    {
        var cruces32 = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        SimulateWinner(cruces32);
        var cruces16 = _service.GenerateNextRound(cruces32, _estadios, 123);
        SimulateWinner(cruces16);
        var cuartos = _service.GenerateNextRound(cruces16, _estadios, 123);
        SimulateWinner(cuartos);
        var semis = _service.GenerateNextRound(cuartos, _estadios, 123);
        SimulateWinner(semis);
        var finals = _service.GenerateNextRound(semis, _estadios, 123);
        Assert.AreEqual(TournamentPhase.Final, finals[0].Phase);
        Assert.AreEqual(1, finals.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldHaveGroupA_WithNames_A1_to_A8()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        var crucesA = cruces.Where(c => c.Nombre.StartsWith("A")).ToList();
        Assert.AreEqual(8, crucesA.Count, "Debe haber 8 cruces del grupo A");
        for (int i = 0; i < 8; i++)
            Assert.AreEqual($"A{i + 1}", crucesA[i].Nombre);
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldHaveCorrectStructure_A_8_B_4_B_4()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        Assert.AreEqual(8, cruces.Count(c => c.Nombre.StartsWith("A")), "8 cruces tipo A");
        Assert.AreEqual(8, cruces.Count(c => c.Nombre.StartsWith("B")), "8 cruces tipo B");
    }

    [TestMethod]
    public void GenerateRoundOf32_NoDuplicateTeams()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        var todosEquipos = new HashSet<string>();
        foreach (var cruce in cruces)
        {
            var home = cruce.Partido.HomeTeam.Name;
            var away = cruce.Partido.AwayTeam.Name;
            Assert.IsFalse(todosEquipos.Contains(home), $"Equipo {home} aparece múltiples veces");
            Assert.IsFalse(todosEquipos.Contains(away), $"Equipo {away} aparece múltiples veces");
            todosEquipos.Add(home);
            todosEquipos.Add(away);
        }
        Assert.AreEqual(32, todosEquipos.Count, "Deben haber exactamente 32 equipos únicos");
    }

    [TestMethod]
    public void GenerateRoundOf32_AllTeamsFromGroups_Simple()
    {
        var grupos = Create12GroupsWithTeams();
        var cruces = _service.GenerateRoundOf32(CreateFixtureWithGroups(grupos), _estadios, 123);

        if (cruces.Count != 16)
            Assert.Fail($"Error: Se generaron {cruces.Count} partidos, pero deberían ser 16");

        bool todosLosEquiposValidos = true;
        string mensajeError = "";
        foreach (var cruce in cruces)
        {
            string local = cruce.Partido.HomeTeam.Name;
            string visitante = cruce.Partido.AwayTeam.Name;
            bool localEncontrado = false;
            bool visitanteEncontrado = false;
            foreach (var grupo in grupos)
            {
                foreach (var equipo in grupo.TeamsInGroup)
                {
                    if (equipo.Name == local) localEncontrado = true;
                    if (equipo.Name == visitante) visitanteEncontrado = true;
                }
            }
            if (!localEncontrado)
            {
                todosLosEquiposValidos = false;
                mensajeError += $"Equipo '{local}' no está en ningún grupo. ";
            }
            if (!visitanteEncontrado)
            {
                todosLosEquiposValidos = false;
                mensajeError += $"Equipo '{visitante}' no está en ningún grupo. ";
            }
        }
        Assert.IsTrue(todosLosEquiposValidos, mensajeError);
    }

    [TestMethod]
    public void GenerateRoundOf32_AllCrucesHaveValidMatch()
    {
        var cruces = _service.GenerateRoundOf32(CreateDefaultFixture(), _estadios, 123);
        foreach (var cruce in cruces)
        {
            Assert.IsNotNull(cruce.Partido, "El partido no puede ser nulo");
            Assert.IsNotNull(cruce.Partido.HomeTeam, "El equipo local no puede ser nulo");
            Assert.IsNotNull(cruce.Partido.AwayTeam, "El equipo visitante no puede ser nulo");
            Assert.IsNotNull(cruce.Partido.Stadium, "El estadio no puede ser nulo");
            Assert.AreNotEqual(cruce.Partido.HomeTeam.Name, cruce.Partido.AwayTeam.Name, "No pueden jugar contra sí mismos");
        }
    }
}