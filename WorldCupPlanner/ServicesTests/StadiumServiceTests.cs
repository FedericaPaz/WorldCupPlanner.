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
        Assert.ThrowsException<ArgumentNullException>(() => _service.AddStadium(null, "Montevideo", "Estadio de Peniarol", 40600));
    }
    
    [TestMethod]
    public void AddStadium_WithNullCity_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.AddStadium("Campeon del Siglo", null, "Estadio de Peniarol", 40600));
    }
    
    
}