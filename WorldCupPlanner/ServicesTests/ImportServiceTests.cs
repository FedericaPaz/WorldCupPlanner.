using Domain;
using Repository;
using Services;
using ServicesTests.Stubs;
using ServicesTests.TestHelpers;

namespace ServicesTests;

[TestClass]
public class ImportServiceTests
{
    private TeamRepository _teamRepository;
    private TeamServiceStub _teamService;
    private ImportService _service;
    
    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new TeamRepository();
        _teamService = new TeamServiceStub(_teamRepository);
        _service = new ImportService(_teamRepository, _teamService);
    }
    
    
    [TestMethod]
    public void ImportTeams_WithInvalidCsv_ShouldNotSaveTeams()
    { 
        string csv = """
                     Nombre,Confederacion,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        var response = _service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(0, _teamRepository.GetAll().Count);
    }
    
    [TestMethod]
    public void ImportTeams_WithValidCsv_ShouldSaveTeams()
    {
        string csv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        var response = _service.ImportTeams(csv);
        if (!response.Success)
        {
            var mensajes = string.Join("\n", response.Errors.Select(e => $"Row {e.RowNumber}: {e.Message}"));
            Assert.Fail($"Import falló:\n{mensajes}");
        }
        Assert.IsTrue(response.Success);
        Assert.AreEqual(48, response.ImportedTeamsCount);
        Assert.AreEqual(48, _teamRepository.GetAll().Count);
    }
    
    [TestMethod]
    public void CreateImportService_WithNullRepository_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ImportService(null, _teamService));
    }
    
    [TestMethod]
    public void ImportTeams_WithInvalidCsv_ShouldReturnErrors()
    {
        string csv = """
                     Nombre,Confederacion,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        var response = _service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(1, response.Errors.Count);
    }
}