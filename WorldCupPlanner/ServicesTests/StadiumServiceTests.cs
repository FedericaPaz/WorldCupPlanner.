using DTOs;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service;
    private StadiumRepository  _repository;

    private StadiumRequest CreateRequest(string stadiumName, string stadiumCity, string? stadiumDescription, int locativeCapacity)
    {
        return new StadiumRequest()
        {
            stadiumName = stadiumName,
            stadiumCity = stadiumCity,
            stadiumDescription = stadiumDescription,
            locativeCapacity = locativeCapacity
        };
    }
    
    [TestInitialize]
    public void Setup()
    {
        _repository = new StadiumRepository();
        _service = new StadiumService(_repository);
    }
    
    [TestMethod]
    public void AddStadium_WithValidData_ShouldNotThrowException()
    {
        var dto = CreateRequest("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium(dto);
        var result = _service.GetStadium(dto.stadiumName);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void AddStadium_WithDuplicateName_ShouldThrowException()
    {
        var dto1 = new StadiumRequest
        {
            stadiumName = "Campeon del Siglo",
            stadiumCity = "Montevideo",
            stadiumDescription = "Estadio de Peniarol",
            locativeCapacity = 40700
        };
        _service.AddStadium(dto1);
        var dto2 = new StadiumRequest
        {
            stadiumName = "Campeon del Siglo",
            stadiumCity = "OtraCiudad",
            stadiumDescription = "OtraDescripcion",
            locativeCapacity = 40600
        };
        Assert.ThrowsException<InvalidOperationException>(() => _service.AddStadium(dto2));
    }
    
    [TestMethod]
    public void AddStadium_WithNullStadium_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.AddStadium(null!));
    }
    
    [TestMethod]
    public void AddStadium_WithNullCity_ShouldThrowException()
    {
        var dto = new StadiumRequest
        {
            stadiumName = "Campeon del Siglo",
            stadiumCity = null!,
            stadiumDescription = "Estadio de Peniarol",
            locativeCapacity = 40700
        };
        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(dto));
    }

    /*
    [TestMethod]
    public void AddStadium_WithCapacityBelowMinimum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>  _service.AddStadium("Campoen del Siglo", "Montevideo", "Estadio de Peniarol", 19999));
    }
    */

    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldNotThrowException()
    {
        var dto = new StadiumRequest
        {
            stadiumName = "Campeon del Siglo",
            stadiumCity = "Montevideo",
            stadiumDescription = "Estadio de Peniarol",
            locativeCapacity = 40700
        };
        _service.AddStadium(dto);
        _service.DeleteStadium("Campeon del Siglo");
    }

    /*
    [TestMethod]
    public void DeleteStadium_WithNonExistingStadium_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.DeleteStadium("Campeon del Siglo"));
    }*/
    
    /*
    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldBeRemovedFromRepository()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.DeleteStadium("Campeon del Siglo");
        var result = _repository.GetStadium("Campeon del Siglo");
        Assert.IsNull(result);
    }*/
    
    [TestMethod]
    public void GetAllStadiums_WithNoStadiums_ShouldReturnEmptyList()
    {
        var result = _service.GetAllStadiums();
        Assert.AreEqual(0, result.Count);
    }
    
    /*
    [TestMethod]
    public void GetAllStadiums_WithMultipleStadiums_ShouldReturnAllStadiums()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium("Centenario", "Montevideo", "Estadio historico", 60000);
        var result = _service.GetAllStadiums();
        Assert.AreEqual(2, result.Count);
    }*/

    [TestMethod]
    public void GetStadium_WithExistingStadium_ShouldReturnStadium()
    {
        var dto = new StadiumRequest
        {
            stadiumName = "Campeon del Siglo",
            stadiumCity = "Montevideo",
            stadiumDescription = "Estadio de Peniarol",
            locativeCapacity = 40700
        };
        _service.AddStadium(dto);
        var result = _service.GetStadium(dto.stadiumName);
        Assert.IsNotNull(result);
        Assert.AreEqual(dto.stadiumName, result.stadiumName);
        Assert.AreEqual(dto.locativeCapacity, result.locativeCapacity);
        Assert.AreEqual(dto.stadiumCity, result.stadiumCity);
    }

    [TestMethod]
    public void GetStadium_WithNonExistingStadium_ShouldReturnNull()
    {
        var result = _service.GetStadium("Campeon del Siglo");
        Assert.IsNull(result);
    }
    
}