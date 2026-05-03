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
    
    
}