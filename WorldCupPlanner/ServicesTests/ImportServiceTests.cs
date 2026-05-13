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
    
    [TestMethod]
    public void CreateImportService_WithNullTeamService_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ImportService(_teamRepository, null));
    }
    
    [TestMethod]
    public void ImportTeams_WithValidCsv_ShouldReturnImportedTeamsCount48()
    {
        string csv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        var response = _service.ImportTeams(csv);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(48, response.ImportedTeamsCount);
    }
    
    [TestMethod]
    public void ImportTeams_WithValidCsv_ShouldClearPreviousTeams()
    {
        string firstCsv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        _service.ImportTeams(firstCsv);
        Assert.AreEqual(48, _teamRepository.GetAll().Count);
        string secondCsv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        var response = _service.ImportTeams(secondCsv);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(48, _teamRepository.GetAll().Count);
    }
    
    [TestMethod]
    public void ImportTeams_WithValidCsv_ShouldSaveThroughSaveImportedTeams()
    {
        string csv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        var response = _service.ImportTeams(csv);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(48, _teamRepository.GetAll().Count);
    }
    
    [TestMethod]
    public void ImportTeams_WithValidCsvButLessThan48Teams_ShouldReturnError()
    {
        string csv = string.Join(
            Environment.NewLine,
            ImportCsvBuilder.CreateValidCsvWith48Teams()
                .Split(Environment.NewLine)
                .Take(48) 
        );
        var response = _service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(1, response.Errors.Count);
        Assert.AreEqual(0, response.Errors[0].RowNumber);
        StringAssert.Contains(response.Errors[0].Message, "exactamente 48 equipos");
    }
    
    [TestMethod]
    public void ImportTeams_WithValidCsvButMoreThan48Teams_ShouldReturnError()
    {
        string csv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        string extraTeam = Environment.NewLine + "Equipo Extra,CONMEBOL,1200";
        csv += extraTeam;
        var response = _service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(1, response.Errors.Count);
        Assert.AreEqual(0, response.Errors[0].RowNumber);
        StringAssert.Contains(response.Errors[0].Message, "exactamente 48 equipos");
    }
    
    [TestMethod]
    public void ImportTeams_WhenTeamServiceAddFails_ShouldReturnError()
    {
        var teamServiceWithError = new TeamServiceStubWithError();
        var service = new ImportService(_teamRepository, teamServiceWithError);
        string csv = ImportCsvBuilder.CreateValidCsvWith48Teams();
        var response = service.ImportTeams(csv);
        Assert.IsFalse(response.Success);
        Assert.AreEqual(1, response.Errors.Count);
        Assert.AreEqual(0, response.Errors[0].RowNumber);
        Assert.AreEqual("Error al agregar equipo", response.Errors[0].Message);
        Assert.AreEqual(0, _teamRepository.GetAll().Count);
    }
}