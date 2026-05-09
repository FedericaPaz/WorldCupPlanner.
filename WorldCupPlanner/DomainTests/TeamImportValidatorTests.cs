using Domain;

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
            "RankingFIFA"
        };
        List<ImportError> errors = validator.ValidateHeaders(headers);
        Assert.AreEqual(0, errors.Count);
    }
}