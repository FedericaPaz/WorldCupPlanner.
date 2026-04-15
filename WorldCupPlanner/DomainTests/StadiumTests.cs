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

    [TestMethod]
    public void createStadium_WithInvalidName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("", "Montevideo", "Estadio de Peniarol", 40700));
    }

    [TestMethod]
    public void createStadium_WithInvalidCity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", "", "Estadio de Peniarol", 40700));
    }
    
}