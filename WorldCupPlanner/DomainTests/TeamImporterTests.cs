using Domain;

namespace DomainTests;

[TestClass]
public class TeamImporterTests
{
    private TeamImporter importer;
    
    [TestInitialize]
    public void Initialize()
    {
         importer = new TeamImporter(
            new CsvParser(),           
            new TeamImportValidator(),
            new TeamFactory()        
        );
    }
    
    [TestMethod]
    public void Import_WithValidCsv_ShouldImportTeams()
    {
        string csv = """
                     Nombre,Confederación,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        ImportResult result = importer.Import(csv);
        Assert.AreEqual(1, result.Teams.Count);
        Assert.AreEqual("Uruguay", result.Teams[0].Name);
        Assert.AreEqual("CONMEBOL", result.Teams[0].Confederation.Name);
        Assert.AreEqual(1800, result.Teams[0].FifaRanking);
    }
    
    [TestMethod]
    public void Import_WithInvalidHeaders_ShouldReturnErrors()
    {
        string csv = """
                     Nombre,Confederacion,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        ImportResult result = importer.Import(csv);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.Errors.Count);
    }
    
    [TestMethod]
    public void Import_WithInvalidRow_ShouldReturnErrors()
    {
        string csv = """
                     Nombre,Confederación,RankingFIFA
                     Uruguay,AUF,1800
                     """;
        ImportResult result = importer.Import(csv);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(0, result.Teams.Count);
    }
    
    [TestMethod]
    public void Import_WithDuplicatedTeamName_ShouldReturnErrors()
    {
        string csv = """
                     Nombre,Confederación,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     Uruguay,CONMEBOL,1700
                     """;
        ImportResult result = importer.Import(csv);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(1, result.Teams.Count);
    }
}