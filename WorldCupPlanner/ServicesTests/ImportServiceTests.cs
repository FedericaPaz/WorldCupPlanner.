using Repository;
using Services;
using ServicesTests.TestHelpers;

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
    
    [TestMethod]
    public void ImportTeams_WithValidCsv_ShouldSaveTeams()
    {
        var teamRepository = new TeamRepository();
        var service = new ImportService(teamRepository);
        string csv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        var response = service.ImportTeams(csv);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(48, response.ImportedTeamsCount);
        Assert.AreEqual(48, teamRepository.GetAll().Count);
    }
    
    [TestMethod]
    public void CreateImportService_WithNullRepository_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ImportService(null));
    }
    
    [TestMethod]
    public void ImportTeams_WithInvalidCsv_ShouldReturnErrors()
    {
        var teamRepository = new TeamRepository();
        var service = new ImportService(teamRepository);
        string csv = """
                     Nombre,Confederacion,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        var response = service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(1, response.Errors.Count);
    }
}