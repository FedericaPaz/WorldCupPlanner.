using Domain;

namespace DomainTests;

[TestClass]
public class ParsedCsvTests
{
    [TestMethod]
    public void AddRow_WithValidRow_ShouldAddRow()
    {
        var parsedCsv = new ParsedCsv();
        var row = new ParsedCsvRow(2, new Dictionary<string, string>
        {
            ["Nombre"] = "Uruguay"
        });
        parsedCsv.AddRow(row);
        Assert.AreEqual(1, parsedCsv.Rows.Count);
        Assert.AreEqual(row, parsedCsv.Rows[0]);
    }
}