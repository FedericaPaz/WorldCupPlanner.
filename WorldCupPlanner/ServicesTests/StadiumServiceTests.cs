using Domain;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service;
    private StadiumRepository  _repository;

    [TestInitialize]
    public void Setup()
    {
        _repository = new StadiumRepository();
        _service = new StadiumService(_repository);
    }

    [TestMethod]
    public void AddStadium_WithValidData_ShouldNotThrowException()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
    }

    [TestMethod]
    public void AddStadium_WithDuplicateName_ShouldThrowException()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.AddStadium("Campeon del Siglo", "OtraCiudad", "OtraDescripcion", 40600));
    }
    
    [TestMethod]
    public void AddStadium_WithNullStadium_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.AddStadium(null, "Montevideo", "Estadio de Peniarol", 40700));
    }
    
    [TestMethod]
    public void AddStadium_WithNullCity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.AddStadium("Campeon del Siglo", null, "Estadio de Peniarol", 40700));
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
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
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
    
    
    
}