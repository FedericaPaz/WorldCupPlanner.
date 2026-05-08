using Domain;

namespace DomainTests;

[TestClass]
public class ParsedCsvRowTests
{
    [TestMethod]
    public void GetValue_WithExistingColumn_ShouldReturnValue()
    {
        var values = new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay",
            ["Confederación"] = "CONMEBOL",
            ["RankingFIFA"] = "1800"
        };
        var row = new ParsedCsvRow(2, values);
        Assert.AreEqual("Uruguay", row.GetValue("Nombre"));
    }
    
    [TestMethod]
    public void GetValue_WithMissingColumn_ShouldThrowException()
    {
        var values = new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay"
        };
        var row = new ParsedCsvRow(2, values);
        Assert.ThrowsException<ArgumentException>(() => row.GetValue("RankingFIFA"));
    }
}