using Domain.Import;

namespace DomainTests;

[TestClass]
public class TeamImportValidatorTests
{
    [TestMethod]
    public void ValidateHeaders_WithValidHeaders_ShouldNotReturnErrors()
    {
        var validator = new TeamImportValidator();
        var headers = new List<string>
        {
            "Nombre",
            "Confederación",
            "RankingFIFA",
            "Bandera"
        };
        List<ImportError> errors = validator.ValidateHeaders(headers);
        Assert.AreEqual(0, errors.Count);
    }

    [TestMethod]
    public void ValidateHeaders_WithInvalidHeaders_ShouldReturnErrors()
    {
        var validator = new TeamImportValidator();
        var headers = new List<string>
        { 
            "Nombre","Confederacion", "RankingFIFA"
        };
        List<ImportError> errors = validator.ValidateHeaders(headers);
        Assert.AreEqual(1, errors.Count);
    }

    [TestMethod]
    public void ValidateRow_WithDuplicatedName_ShouldReturnError()
    {
        var validator = new TeamImportValidator();
        var row = new ParsedCsvRow(3, new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay",
            ["Confederación"] = "CONMEBOL",
            ["RankingFIFA"] = "1800",
            ["Bandera"] = ""
        });
        var importedNames = new HashSet<string> { "Uruguay" };
        List<ImportError> errors = validator.ValidateRow(row, importedNames);
        Assert.AreEqual(1, errors.Count);
    }

    [TestMethod]
    public void ValidateRow_WithInvalidConfederation_ShouldReturnError()
    {
        var validator = new TeamImportValidator();
        var row = new ParsedCsvRow(2, new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay",
            ["Confederación"] = "AUF",
            ["RankingFIFA"] = "1800",
            ["Bandera"] = ""
        });
        List<ImportError> errors = validator.ValidateRow(row, new HashSet<string>());
        Assert.AreEqual(1, errors.Count);
    }

    [TestMethod]
    public void ValidateRow_WithInvalidRanking_ShouldReturnError()
    {
        var validator = new TeamImportValidator();
        var row = new ParsedCsvRow(2, new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay", ["Confederación"] = "CONMEBOL", ["RankingFIFA"] = "100", ["Bandera"] = ""
        });
        List<ImportError> errors = validator.ValidateRow(row, new HashSet<string>());
        Assert.AreEqual(1, errors.Count);
    }
    
    [TestMethod]
    public void ValidateRow_WithAllValidData_ShouldNotReturnErrors()
    {
        var validator = new TeamImportValidator();
        var row = new ParsedCsvRow(1, new Dictionary<string, string>
        {
            ["Nombre"] = "Argentina",
            ["Confederación"] = "CONMEBOL",
            ["RankingFIFA"] = "1800", 
            ["Bandera"] = ""
        });
        
        List<ImportError> errors = validator.ValidateRow(row, new HashSet<string>());
        
        Assert.AreEqual(0, errors.Count);
    }
    
    [TestMethod]
    public void ValidateRow_WithNonNumericRanking_ShouldReturnError()
    {
        var validator = new TeamImportValidator();
        var row = new ParsedCsvRow(2, new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay", 
            ["Confederación"] = "CONMEBOL", 
            ["RankingFIFA"] = "NoEsUnNumero", 
            ["Bandera"] = ""
        });
        
        List<ImportError> errors = validator.ValidateRow(row, new HashSet<string>());
        
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors[0].Message.Contains("El ranking FIFA debe estar entre"));
    }
    
    [TestMethod]
    public void ValidateRow_WithInvalidBase64Flag_ShouldReturnError()
    {
        var validator = new TeamImportValidator();
        var row = new ParsedCsvRow(3, new Dictionary<string, string>
        {
            ["Nombre"] = "Brasil",
            ["Confederación"] = "CONMEBOL",
            ["RankingFIFA"] = "1900",
            ["Bandera"] = "esto-no-es-base64-valido-!!!"
        });

        List<ImportError> errors = validator.ValidateRow(row, new HashSet<string>());

        Assert.AreEqual(1, errors.Count);
        Assert.AreEqual("El formato de la bandera en esta fila es inválido.", errors[0].Message);
    }
}