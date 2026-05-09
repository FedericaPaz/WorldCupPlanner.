using Domain;

namespace DomainTests;

[TestClass]
public class ImportErrorTests
{
    [TestMethod]
    public void CreateImportError_WithValidData_ShouldCreateImportError()
    {
        var error = new ImportError(2, "Error");
        Assert.AreEqual(2, error.RowNumber);
        Assert.AreEqual("Error", error.Message);
    }
}