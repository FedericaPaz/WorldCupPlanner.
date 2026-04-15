using Domain;
namespace DomainTests;

[TestClass]
public class StadiumTests
{
    [TestMethod]
    public void createStadium_WithValidData_ShouldSucceed()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        Assert.IsNotNull(stadium);
    }
}