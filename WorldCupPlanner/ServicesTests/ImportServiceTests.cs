using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class ImportServiceTests
{
    [TestMethod]
    public void ImportTeams_WithInvalidCsv_ShouldNotSaveTeams()
    {
        var teamRepository = new TeamRepository();
        var service = new ImportService(teamRepository);
        string csv = """
                     Nombre,Confederacion,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        var response = service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(0, teamRepository.GetAll().Count);
    }
}