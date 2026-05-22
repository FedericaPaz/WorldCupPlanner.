using Domain.Entities;
using Repository;

namespace RepositoryTests;

[TestClass]
public class TeamRepositoryTests
{
    private TeamRepository _teamRepository;
        
    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new TeamRepository();
    }
    
    [TestMethod]
    public void AddTeam_ShouldStoreTeam()
    {
        var team = CreateTeam("Uruguay");

        _teamRepository.Add(team);

        var teams = _teamRepository.GetAll();

        Assert.AreEqual(1, teams.Count);
        Assert.AreEqual(team, teams[0]);
    }
    
    [TestMethod]
    public void AddTeam_Null_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _teamRepository.Add(null));
    }
    
    [TestMethod]
    public void Get_ExistingTeam_ShouldReturnTeam()
    {
        var team = CreateTeam("Uruguay");

        _teamRepository.Add(team);

        var result = _teamRepository.Get("Uruguay");

        Assert.AreEqual(team, result);
    }
    
    [TestMethod]
    public void Get_NonExistingTeam_ShouldReturnNull()
    {
        var result = _teamRepository.Get("Argentina");

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void GivenMultipleTeams_WhenGetAll_ShouldReturnAllTeams()
    {
        var team1 = CreateTeam("Uruguay");
        var team2 = CreateTeam("Argentina");

        _teamRepository.Add(team1);
        _teamRepository.Add(team2);

        var teams = _teamRepository.GetAll();

        Assert.AreEqual(2, teams.Count);
        Assert.IsTrue(teams.Contains(team1));
        Assert.IsTrue(teams.Contains(team2));
    }
    
    [TestMethod]
    public void DeleteTeam_WhenExists_ShouldRemoveTeam()
    {
        var team = CreateTeam("Uruguay");

        _teamRepository.Add(team);
        _teamRepository.Delete(team);

        Assert.IsNull(_teamRepository.Get("Uruguay"));
    }

    [TestMethod]
    public void DeleteTeam_WhenNonExists_ShouldThrowException()
    {
        var team = CreateTeam("Uruguay");

        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Delete(team));
    }
    
    [TestMethod]
    public void UpdateTeam_WhenExists_ShouldUpdateTeam()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var updatedTeam = new Team(team.Name, new Confederation("CONMEBOL"), 1000);

        _teamRepository.Update(updatedTeam);

        var result = _teamRepository.Get(team.Name);

        Assert.IsNotNull(result);
        Assert.AreEqual("CONMEBOL", result.Confederation.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }

    [TestMethod]
    public void UpdateTeam_WhenNonExists_ShouldThrowException()
    {
        var team = CreateTeam("Uruguay");

        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Update(team));
    }
    
    [TestMethod]
    public void GetAll_ShouldReturnCopy_NotInternalList()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var list = _teamRepository.GetAll();
        list.Clear();

        var result = _teamRepository.GetAll();

        Assert.AreEqual(1, result.Count);
    }
    
    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("AFC"), 500);
    }
}