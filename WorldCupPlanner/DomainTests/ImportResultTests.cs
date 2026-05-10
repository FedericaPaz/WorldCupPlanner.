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
    
    [TestMethod]
    public void AddError_WithValidError_ShouldAddError()
    {
        var result = new ImportResult();
        var error = new ImportError(2, "Error");
        result.AddError(error);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(error, result.Errors[0]);
        Assert.IsFalse(result.Success);
    }
    
    [TestMethod]
    public void AddTeam_WithValidTeam_ShouldAddTeam()
    {
        var result = new ImportResult();
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        result.AddTeam(team);
        Assert.AreEqual(1, result.Teams.Count);
        Assert.AreEqual(team, result.Teams[0]);
    }
}