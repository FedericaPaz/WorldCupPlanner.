using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;

namespace DomainTests;


[TestClass]
public class ConfederationTests
{
    [TestMethod]
    public void CreateConfederation_WithValidName_ShouldSucceed()
    {
        var condeferation = new Confederation("UEFA");
        Assert.AreEqual("UEFA", condeferation.Name);
    }

    [TestMethod]
    public void CreateConfederation_NullName_ShouldFail()
    {
        Assert.ThrowsException<ArgumentNullException>(()=> new Confederation(null));
    }

    [TestMethod]
    public void CreateConfederation_WithEmptyName_ShouldFail()
    {
        Assert.ThrowsException<ArgumentException>(()=> new  Confederation(""));
        //Assert.ThrowsException<ArgumentException>(()=> new  Confederation(" "));
    }

    [TestMethod]
    public void CreateConfederation_WithWhiteSpaceName_ShouldFail()
    {
        Assert.ThrowsException<ArgumentException>(()=> new  Confederation("  "));
    }
    
    [TestMethod]
    public void CreateConfederation_WithLowercaseName_ShouldNormalizeToUppercase()
    {
        var confederation = new Confederation("uefa");
        Assert.AreEqual("UEFA", confederation.Name);
    }
    
    [TestMethod]
    public void CreateConfederation_WithNameWithSpaces_ShouldTrimAndNormalize()
    {
        var confederation = new Confederation("  conmebol  ");
        Assert.AreEqual("CONMEBOL", confederation.Name);
    }

    [TestMethod]
    public void CreateConfederation_WithInvalidName_ShouldFail()
    {
        Assert.ThrowsException<ArgumentException>(()=> new Confederation("UUEEF_FAAA"));
        Assert.ThrowsException<ArgumentException>(()=> new Confederation("12345"));
    }
    
    
}