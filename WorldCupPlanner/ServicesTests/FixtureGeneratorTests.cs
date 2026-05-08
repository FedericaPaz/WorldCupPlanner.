using Repository;
using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class FixtureGeneratorTests
{
    private TeamRepository _teamRepository;    
    private StadiumRepository _stadiumRepository;
    private FixtureGenerator _generator;
    private TournamentConfig _config;
    
    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new TeamRepository();
        _stadiumRepository = new StadiumRepository();
        _generator = new FixtureGenerator(_teamRepository, _stadiumRepository);
        _config = new TournamentConfig();
        Match.ResetIdCounter();
    }
    
    private void AddTeams(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            var team = new Team($"Team_{i}", new Confederation("UEFA"), 500 + i);
            _teamRepository.Add(team);
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
}