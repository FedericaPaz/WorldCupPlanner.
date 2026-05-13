using Domain;

namespace DomainTests;

[TestClass]
public class CsvParserTests
{
    [TestMethod]
    public void Parse_WithValidCsv_ShouldParseHeadersAndRows()
    {
        string csv = """
                     Nombre,Confederación,RankingFIFA
                     Uruguay,CONMEBOL,1800
                     """;
        var parser = new CsvParser();
        ParsedCsv parsedCsv = parser.Parse(csv);
        Assert.AreEqual(3, parsedCsv.Headers.Count);
        Assert.AreEqual(1, parsedCsv.Rows.Count);
        Assert.AreEqual("Uruguay", parsedCsv.Rows[0].GetValue("Nombre"));
    }
}