using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class CruceGeneratorServiceTests
{
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
        DateTime fechaPartido = DateTime.Now.AddHours(-2);
        DateTime fechaActual  =  DateTime.Now;
        Match match = new Match(fechaPartido,
            new Stadium("Centenario", "Montevideo", "Desc", 50000),
            homeTeam, awayTeam, groupLetter, new ClockStub(fechaActual));

        match.SetMatchResult(homeGoals, awayGoals);
        return match;
    }
    
    private void SimulateWinner(List<Cruce> cruces)
    {
        foreach (Cruce cruce in cruces)
            cruce.Partido.SetMatchResult(2, 0);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldGenerate16Cruces()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Group> grupos = Create12GroupsWithTeams();
        List<Stadium> estadios = CreateStadiums();
        List<Cruce> cruces = service.GenerateRoundOf32(grupos, estadios, 123);
        Assert.AreEqual(16, cruces.Count);
    }

    [TestMethod]
    public void GenerateRoundOf32_WithEmptyGroups_ShouldThrowException()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Stadium> estadios = CreateStadiums();
        Assert.ThrowsException<ArgumentException>(() => service.GenerateRoundOf32(new List<Group>(), estadios, 123));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithNullGroups_ShouldThrowException()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Stadium> estadios = CreateStadiums();
        Assert.ThrowsException<ArgumentNullException>(() => service.GenerateRoundOf32(null, estadios, 123));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldUseQualifiedTeams()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Group> grupos = Create12GroupsWithTeams();
        List<Cruce> cruces = service.GenerateRoundOf32(grupos, CreateStadiums(), 123);
        Assert.IsTrue(grupos.SelectMany(g => g.TeamsInGroup)
            .Any(t => t.Name == cruces[0].Partido.HomeTeam.Name));
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldGenerateRoundOf32Phase()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.RoundOf32, cruces[0].Phase);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_ShouldRotateStadiums()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.AreEqual("Estadio 1", cruces[0].Partido.Stadium.StadiumName);
        Assert.AreEqual("Estadio 2", cruces[1].Partido.Stadium.StadiumName);
    }
    
    [TestMethod]
    public void GenerateRoundOf32_WithSameSeed_ShouldGenerateSameOrder()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Cruce> cruces1 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        List<Cruce> cruces2 =service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.AreEqual(cruces1[0].Partido.HomeTeam.Name, cruces2[0].Partido.HomeTeam.Name);
    }
    
    [TestMethod]
    public void GenerateNextRound_ShouldGenerateRoundOf16()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Cruce> cruces32 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        SimulateWinner(cruces32);
        List<Cruce> cruces16 = service.GenerateNextRound(cruces32, CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.RoundOf16, cruces16[0].Phase);
        Assert.AreEqual(8, cruces16.Count);
    }
    
    [TestMethod]
    public void GenerateNextRound_WithoutWinners_ShouldThrowException()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Cruce> cruces = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        Assert.ThrowsException<InvalidOperationException>(() => service.GenerateNextRound(cruces, CreateStadiums(), 123));
    }
    
    [TestMethod]
    public void GenerateNextRound_FromRoundOf16_ShouldGenerateQuarterFinals()
    {
        CruceGeneratorService service = new CruceGeneratorService();
        List<Cruce> cruces32 = service.GenerateRoundOf32(Create12GroupsWithTeams(), CreateStadiums(), 123);
        SimulateWinner(cruces32);
        List<Cruce> cruces16 = service.GenerateNextRound(cruces32, CreateStadiums(), 123);
        SimulateWinner(cruces16);
        List<Cruce> cuartos = service.GenerateNextRound(cruces16, CreateStadiums(), 123);
        Assert.AreEqual(TournamentPhase.QuarterFinals, cuartos[0].Phase);
        Assert.AreEqual(4, cuartos.Count);
    }
}