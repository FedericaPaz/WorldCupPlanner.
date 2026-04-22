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
        Assert.AreEqual("Campeon del Siglo", stadium.StadiumName);
        Assert.AreEqual("Montevideo", stadium.CityName);
        Assert.AreEqual("Estadio de Peniarol", stadium.StadiumDescription);
        Assert.AreEqual(40700, stadium.LocativeCapacity);
    }
    
    [TestMethod]
    public void createStadium_WithoutDescription_ShouldSucceed()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "", 40700);
        Assert.IsNotNull(stadium);
        Assert.AreEqual("", stadium.StadiumDescription);
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

    [TestMethod]
    public void createStadium_WithZeroLocativeCapacity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 0));
    }
    
    [TestMethod]
    public void createStadium_WithNegativeLocativeCapacity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", -1));
    }
    
    [TestMethod]
    public void createStadium_WithCapacityLessThan20000_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 19999));
    }

    [TestMethod]
    public void createStadium_WithTooLongName_ShouldThrowException()
    {
        string name = new string('a', 81);
        Assert.ThrowsException<ArgumentException>(() => new Stadium(name, "Montevideo", "Estadio de Peniarol", 40700));
    }

    [TestMethod]
    public void createStadium_WithTooLongCity_ShouldThrowException()
    {
        string city = new string('a', 61);
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", city, "Estadio de Peniarol", 40700));
    }

    [TestMethod]
    public void createStadium_WithTooLongDescpription_ShouldThrowException()
    {
        string description = new string('a', 401);
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", "Montevideo", description, 40700));
    }
}