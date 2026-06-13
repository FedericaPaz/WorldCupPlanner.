using Domain.Entities;

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
    
    [TestMethod]
    public void createStadium_WithNullDescription_ShouldSetEmptyDescription()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", null, 40700);
        Assert.AreEqual(string.Empty, stadium.StadiumDescription);
    }

    [TestMethod]
    public void createStadium_WithWhitespaceName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("   ", "Montevideo", "Descripcion", 40700));
    }

    [TestMethod]
    public void createStadium_WithWhitespaceCity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Stadium("Campeon del Siglo", "   ", "Descripcion", 40700));
    }

    [TestMethod]
    public void createStadium_WithMinimumValidCapacity_ShouldSucceed()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Descripcion", 20000);
        Assert.AreEqual(20000, stadium.LocativeCapacity);
    }

    [TestMethod]
    public void createStadium_WithMaximumValidNameLength_ShouldSucceed()
    {
        string name = new string('a', 80);
        var stadium = new Stadium(name, "Montevideo", "Descripcion", 40700);
        Assert.AreEqual(name, stadium.StadiumName);
    }

    [TestMethod]
    public void createStadium_WithMaximumValidCityLength_ShouldSucceed()
    {
        string city = new string('a', 60);
        var stadium = new Stadium("Campeon del Siglo", city, "Descripcion", 40700);
        Assert.AreEqual(city, stadium.CityName);
    }

    [TestMethod]
    public void createStadium_WithMaximumValidDescriptionLength_ShouldSucceed()
    {
        string description = new string('a', 400);
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", description, 40700);
        Assert.AreEqual(description, stadium.StadiumDescription);
    }

    [TestMethod]
    public void createStadium_WithSpaces_ShouldTrimValues()
    {
        var stadium = new Stadium("  Campeon del Siglo  ", "  Montevideo  ", "  Descripcion  ", 40700);
        Assert.AreEqual("Campeon del Siglo", stadium.StadiumName);
        Assert.AreEqual("Montevideo", stadium.CityName);
        Assert.AreEqual("Descripcion", stadium.StadiumDescription);
    }

    [TestMethod]
    public void updateStadiumName_WithValidName_ShouldUpdate()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Descripcion", 40700);
        stadium.StadiumName = "Centenario";
        Assert.AreEqual("Centenario", stadium.StadiumName);
    }

    [TestMethod]
    public void updateCityName_WithValidCity_ShouldUpdate()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Descripcion", 40700);
        stadium.CityName = "Canelones";
        Assert.AreEqual("Canelones", stadium.CityName);
    }

    [TestMethod]
    public void updateDescription_WithValidDescription_ShouldUpdate()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Descripcion", 40700);
        stadium.StadiumDescription = "Nueva descripcion";
        Assert.AreEqual("Nueva descripcion", stadium.StadiumDescription);
    }

    [TestMethod]
    public void updateCapacity_WithValidCapacity_ShouldUpdate()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Descripcion", 40700);
        stadium.LocativeCapacity = 50000;
        Assert.AreEqual(50000, stadium.LocativeCapacity);
    }
}