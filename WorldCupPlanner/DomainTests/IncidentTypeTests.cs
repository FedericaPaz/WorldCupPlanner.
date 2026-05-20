using Domain.Enums;

namespace DomainTests;

[TestClass]
public class IncidentTypeTests
{
    [TestMethod]
    public void IncidentType_CheckingIfYellowCardExists_ReturnsTrue()
    {
        bool exists = Enum.IsDefined(typeof(IncidentType), IncidentType.YellowCard);
        Assert.IsTrue(exists);
    }
    
    [TestMethod]
    public void IncidentType_CheckingIfRedCardExists_ReturnsTrue()
    {
        bool exists = Enum.IsDefined(typeof(IncidentType), IncidentType.RedCard);
        Assert.IsTrue(exists);
    }
}