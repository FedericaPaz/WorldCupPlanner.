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
    
    [TestMethod]
    public void MaxTeams_ShouldReturnCorrectValueForEachConfederation()
    {
        var uefa = new Confederation("UEFA");
        var conmebol = new Confederation("CONMEBOL");
        var concacaf = new Confederation("CONCACAF");
        var caf = new Confederation("CAF");
        var afc = new Confederation("AFC");
        var ofc = new Confederation("OFC");
    
        Assert.AreEqual(16, uefa.MaxTeams);
        Assert.AreEqual(7, conmebol.MaxTeams);
        Assert.AreEqual(7, concacaf.MaxTeams);
        Assert.AreEqual(9, caf.MaxTeams);
        Assert.AreEqual(8, afc.MaxTeams);
        Assert.AreEqual(1, ofc.MaxTeams);
    }
    
    [TestMethod]
    public void GetMaxTeamsForName_ShouldReturnCorrectValue()
    {
        Assert.AreEqual(16, Confederation.GetMaxTeamsForName("UEFA"));
        Assert.AreEqual(16, Confederation.GetMaxTeamsForName("uefa"));     
        Assert.AreEqual(16, Confederation.GetMaxTeamsForName("  UEFA  ")); 
        Assert.AreEqual(0, Confederation.GetMaxTeamsForName("INVALID"));
        Assert.AreEqual(0, Confederation.GetMaxTeamsForName(""));
        Assert.AreEqual(0, Confederation.GetMaxTeamsForName(null!));
    }
    
    [TestMethod]
    public void TwoConfederationsWithSameName_ShouldBeEqual()
    {
        var conf1 = new Confederation("UEFA");
        var conf2 = new Confederation("UEFA");
        var conf3 = new Confederation("CONMEBOL");
    
        Assert.AreEqual(conf1, conf2);
        Assert.AreNotEqual(conf1, conf3);
    }
}