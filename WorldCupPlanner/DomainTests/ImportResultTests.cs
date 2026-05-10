using Domain;

namespace DomainTests;

[TestClass]
public class ImportResultTests
{
    [TestMethod]
    public void CreateImportResult_WhenEmpty_ShouldBeSuccessful()
    {
        var result = new ImportResult();
        Assert.AreEqual(0, result.Teams.Count);
        Assert.AreEqual(0, result.Errors.Count);
        Assert.IsTrue(result.Success);
    }
}