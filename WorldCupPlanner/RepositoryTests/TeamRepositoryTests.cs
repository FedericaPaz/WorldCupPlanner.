using Domain;
using Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepositoryTests;

[TestClass]
public class TeamRepositoryTests
{   
    [TestMethod]
    public void AddTeam_ShouldStoreTeam()
    {
        var repository = new TeamRepository();
        var conf = new Confederation("AFC");
        var team = new Team("Uruguay", conf, 500);

        repository.Add(team);

        var teams = repository.GetAll();

        Assert.HasCount(1, teams);
        Assert.AreEqual(team, teams[0]);
    }
}