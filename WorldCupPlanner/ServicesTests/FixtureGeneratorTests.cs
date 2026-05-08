using Repository;
using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class FixtureGeneratorTests
{
    private TeamRepository _teamRepository;    
    private StadiumRepository _stadiumRepository;
    private FixtureRepository _fixtureRepository;
    private FixtureGenerator _generator;
    private TournamentConfig _config;
    
    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new TeamRepository();
        _stadiumRepository = new StadiumRepository();
        _fixtureRepository = new FixtureRepository();
        _generator = new FixtureGenerator(_teamRepository, _stadiumRepository,_fixtureRepository);
        _config = new TournamentConfig();
        Match.ResetIdCounter();
    }
    
    private void AddTeams(int count)
    {
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7), ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };
    
        int teamIndex = 1;
        int rankingBase = 500;
        foreach (var (name, limit) in confederations)
        {
            int teamsToAdd = Math.Min(limit, count - teamIndex + 1);
            for (int i = 1; i <= teamsToAdd && teamIndex <= count; i++)
            {
                var team = new Team($"Team_{teamIndex}", new Confederation(name), rankingBase + teamIndex);
                _teamRepository.Add(team);
                teamIndex++;
            }
        }
    }
    
    private void AddStadiums(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            var stadium = new Stadium($"Estadio_{i}", $"Ciudad_{i}", "", 50000);
            _stadiumRepository.AddStadium(stadium);
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithLessThan48Teams_ThrowsException()
    {
        AddTeams(40);
        AddStadiums(4);
        try
        {
            _generator.GenerateGroupsStageFixture(_config);
            Assert.Fail("Debería lanzar una excepción");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("48 equipos"));
        }
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_WithLessThan4Stadiums_ThrowsException()
    {
        AddTeams(48);     
        AddStadiums(2);  
        try
        {
            _generator.GenerateGroupsStageFixture(_config);
            Assert.Fail("Debería lanzar una excepción");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("4 estadios"));
        }
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_WithValidData_ReturnsFixture()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(_config);
        Assert.IsNotNull(fixture);
        Assert.AreEqual(TournamentPhase.GroupStage, fixture.CurrentPhase);
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_Creates12Groups()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(_config);
        Assert.AreEqual(12, fixture.GetGroups.Count);
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_DistributesTeamsToGroups()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(_config);
        int totalEquipos = 0;
        foreach (var group in fixture.GetGroups)
            totalEquipos += group.TeamsInGroup.Count;
        Assert.AreEqual(48, totalEquipos);
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_EachGroupHas4Teams()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(_config);
        foreach (var group in fixture.GetGroups)
            Assert.AreEqual(4, group.TeamsInGroup.Count);
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_EachGroupHas6Matches()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(_config);
        foreach (var group in fixture.GetGroups)
            Assert.AreEqual(6, group.MatchesInGroup.Count);
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_MatchesHaveCorrectDates()
    {
        var startDate = new DateTime(2026, 6, 1);
        var config = new TournamentConfig(123,123,123,123,startDate, 3,3);
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(config);
        var primerGrupo = fixture.GetGroups[0];
        var partidos = primerGrupo.MatchesInGroup.ToList();
        
        Assert.AreEqual(startDate.Date, partidos[0].DateMatch.Date);
        Assert.AreEqual(startDate.Date, partidos[1].DateMatch.Date);
        
        Assert.AreEqual(startDate.AddDays(3).Date, partidos[2].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(3).Date, partidos[3].DateMatch.Date);
        
        Assert.AreEqual(startDate.AddDays(6).Date, partidos[4].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(6).Date, partidos[5].DateMatch.Date);
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_MatchesHaveDifferentTeams()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture(_config);
        foreach (var group in fixture.GetGroups)
        {
            foreach (var match in group.MatchesInGroup)
            {
                Assert.AreNotEqual(match.HomeTeam.Name, match.AwayTeam.Name, "Local y visitante no pueden ser el mismo equipo");
            }
        }
    }
}