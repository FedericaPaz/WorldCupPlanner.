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

    private List<Group> Create12GroupsWithTeams()
    {
        var grupos = new List<Group>();
        for (char letra = 'A'; letra <= 'L'; letra++)
        {
            var grupo = new Group(letra);
            for (int i = 0; i < 4; i++)
            {
                var equipo = new Team($"Equipo{letra}{i + 1}", new Confederation("UEFA"), 300 + i);
                grupo.AddTeamToGroup(equipo);
            }
            CreateResults(grupo);
            grupos.Add(grupo);
        }
        return grupos;
    }

    private List<Stadium> CreateStadiums()
    {
        var stadiums = new List<Stadium>();
        stadiums.Add(new Stadium("Estadio 1", "Ciudad 1", "Descripción 1", 50000));
        stadiums.Add(new Stadium("Estadio 2", "Ciudad 2", "Descripción 2", 60000));
        stadiums.Add(new Stadium("Estadio 3", "Ciudad 3", "Descripción 3", 70000));
        stadiums.Add(new Stadium("Estadio 4", "Ciudad 4", "Descripción 4", 80000));
        return stadiums;
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
        var service = new CrucesGeneratorService(ConfigTest);
        List<Group> grupos = Create12GroupsWithTeams();
        List<Stadium> estadios = CreateStadiums();
        List<Cruce> cruces = service.GenerateRoundOf32(grupos, estadios, 123);
        Assert.AreEqual(16, cruces.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_WithEmptyGroups_ShouldThrowException()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Stadium> estadios = CreateStadiums();
        Assert.ThrowsException<ArgumentException>(() => service.GenerateRoundOf32(new List<Group>(), estadios, 123));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithNullGroups_ShouldThrowException()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Stadium> estadios = CreateStadiums();
        Assert.ThrowsException<ArgumentNullException>(() => service.GenerateRoundOf32(null, estadios, 123));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithEmptyStadiums_ShouldThrowException()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Group> grupos = Create12GroupsWithTeams();
        Assert.ThrowsException<ArgumentException>(() => service.GenerateRoundOf32(grupos, new List<Stadium>(), 123));
    } 
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldUseQualifiedTeams()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Group> grupos = Create12GroupsWithTeams();
        List<Cruce> cruces = service.GenerateRoundOf32(grupos, CreateStadiums(), 123);
        Assert.IsTrue(grupos.SelectMany(g => g.TeamsInGroup)
            .Any(t => t.Name == cruces[0].Partido.HomeTeam.Name));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithLessThan12Groups_ShouldThrowException()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Group> grupos = Create12GroupsWithTeams().Take(6).ToList();
        List<Stadium> estadios = CreateStadiums();
        Assert.ThrowsException<ArgumentException>(() => service.GenerateRoundOf32(grupos, estadios, 123));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldGenerateRoundOf32Phase()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.RoundOf32, cruces[0].Phase);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldRotateStadiums()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.AreEqual("Estadio 1", cruces[0].Partido.Stadium.StadiumName);
        Assert.AreEqual("Estadio 2", cruces[1].Partido.Stadium.StadiumName);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithSameSeed_ShouldGenerateSameOrder()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces1 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        List<Cruce> cruces2 =service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.AreEqual(cruces1[0].Partido.HomeTeam.Name, cruces2[0].Partido.HomeTeam.Name);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithDifferentSeed_ShouldGenerateDifferentOrder()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces1 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        List<Cruce> cruces2 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 456);
        
        bool isDifferent = false;
        for (int i = 0; i < cruces1.Count; i++)
        {
            if (cruces1[i].Partido.HomeTeam.Name != cruces2[i].Partido.HomeTeam.Name || cruces1[i].Partido.AwayTeam.Name != cruces2[i].Partido.AwayTeam.Name)
            {
                isDifferent = true;
                break;
            }
        }
        Assert.IsTrue(isDifferent, "Con semillas diferentes debería haber emparejamientos diferentes");
    }
    
    [TestMethod]
    public void GenerateNextRound_ShouldGenerateRoundOf16()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces32 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        SimulateWinner(cruces32);
        List<Cruce> cruces16 = service.GenerateNextRound(cruces32, CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.RoundOf16, cruces16[0].Phase);
        Assert.AreEqual(8, cruces16.Count);
    }
    
    [TestMethod]
    public void GenerateNextRound_WithoutWinners_ShouldThrowException()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.ThrowsException<InvalidOperationException>(() => service.GenerateNextRound(cruces, CreateStadiums(), 123));
    }
    
    [TestMethod]
    public void GenerateNextRound_FromRoundOf16_ShouldGenerateQuarterFinals()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces32 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        SimulateWinner(cruces32);
        List<Cruce> cruces16 = service.GenerateNextRound(cruces32, CreateStadiums(), 123);
        SimulateWinner(cruces16);
        List<Cruce> cuartos = service.GenerateNextRound(cruces16, CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.QuarterFinals, cuartos[0].Phase);
        Assert.AreEqual(4, cuartos.Count);
    }
    
    [TestMethod]
    public void GenerateNextRound_FromQuarters_ShouldGenerateSemiFinals()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces32 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        SimulateWinner(cruces32);
        List<Cruce> cruces16 = service.GenerateNextRound(cruces32, CreateStadiums(), 123);
        SimulateWinner(cruces16);
        List<Cruce> cuartos = service.GenerateNextRound(cruces16, CreateStadiums(), 123);
        SimulateWinner(cuartos);
        List<Cruce> semis = service.GenerateNextRound(cuartos, CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.SemiFinals, semis[0].Phase);
        Assert.AreEqual(2, semis.Count);
    }
    
    [TestMethod]
    public void GenerateNextRound_FromSemis_ShouldGenerateFinals()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces32 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        SimulateWinner(cruces32);
        List<Cruce> cruces16 = service.GenerateNextRound(cruces32, CreateStadiums(), 123);
        SimulateWinner(cruces16);
        List<Cruce> cuartos = service.GenerateNextRound(cruces16, CreateStadiums(), 123);
        SimulateWinner(cuartos);
        List<Cruce> semis = service.GenerateNextRound(cuartos, CreateStadiums(), 123);
        SimulateWinner(semis);
        List<Cruce> finals = service.GenerateNextRound(semis, CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.Final, finals[0].Phase);
        Assert.AreEqual(1, finals.Count);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldHaveGroupA_WithNames_A1_to_A8()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        var crucesA = cruces.Where(c => c.Nombre.StartsWith("A")).ToList();
        Assert.AreEqual(8, crucesA.Count, "Debe haber 8 cruces del grupo A");
        for (int i = 0; i < 8; i++)
            Assert.AreEqual($"A{i + 1}", crucesA[i].Nombre);
    }

    [TestMethod]
    public void GenerateRoundOf32_ShouldHaveCorrectStructure_A_8_B_4_B_4()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        var crucesA = cruces.Count(c => c.Nombre.StartsWith("A"));
        var crucesB = cruces.Count(c => c.Nombre.StartsWith("B"));
        Assert.AreEqual(8, crucesA, "8 cruces tipo A");
        Assert.AreEqual(8, crucesB, "8 cruces tipo B ");
    }

    [TestMethod]
    public void GenerateRoundOf32_NoDuplicateTeams()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
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
        var service = new CrucesGeneratorService(ConfigTest);
        List<Group> grupos = Create12GroupsWithTeams();
        List<Stadium> estadios = CreateStadiums();
        List<Cruce> cruces = service.GenerateRoundOf32(grupos, estadios, 123);
        if (cruces.Count != 16)
        {
            Assert.Fail($"Error: Se generaron {cruces.Count} partidos, pero deberían ser 16");
        }
        bool todosLosEquiposValidos = true;
        string mensajeError = "";
        foreach (Cruce cruce in cruces)
        {
            string local = cruce.Partido.HomeTeam.Name;
            string visitante = cruce.Partido.AwayTeam.Name;
            bool localEncontrado = false;
            bool visitanteEncontrado = false;
            foreach (Group grupo in grupos)
            {
                foreach (Team equipo in grupo.TeamsInGroup)
                {
                    if (equipo.Name == local)
                        localEncontrado = true;
                    if (equipo.Name == visitante)
                        visitanteEncontrado = true;
                }
            }
            if (!localEncontrado)
            {
                todosLosEquiposValidos = false;
                mensajeError = mensajeError + $"Equipo '{local}' no está en ningún grupo. ";
            }
            if (!visitanteEncontrado)
            {
                todosLosEquiposValidos = false;
                mensajeError = mensajeError + $"Equipo '{visitante}' no está en ningún grupo. ";
            }
        }
        Assert.IsTrue(todosLosEquiposValidos, mensajeError);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_AllCrucesHaveValidMatch()
    {
        var service = new CrucesGeneratorService(ConfigTest);
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
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