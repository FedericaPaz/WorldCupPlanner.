using Domain;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service;
    private StadiumRepository  _repository;
    private Stadium _stadium;

    [TestInitialize]
    public void Setup()
    {
        _repository = new StadiumRepository();
        _service = new StadiumService(_repository);
        _stadium = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);

    }

    [TestMethod]
    public void AddStadium_WithValidData_ShouldNotThrowException()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
    }

    [TestMethod]
    public void GivenExistingStadium_AddStadiumWithDuplicateName_ShouldThrowException()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        Assert.ThrowsException<ArgumentException>(() =>
            _service.AddStadium("Campeon del Siglo", "OtraCiudad", "OtraDesc", 40600));
    }

    [TestMethod]
    public void GivenExistingStadium_DeleteStadium_ShouldNotThrowException()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.DeleteStadium(_stadium);
    }

    [TestMethod]
    public void GivingExistingStadium_UpdateStadium_ShouldUpdateStadium()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700 );
        var updatedStadium = new Stadium("Campeon del Siglo","OtraCiudad", "OtraDesc", 40600);
        _service.UpdateStadium(updatedStadium);
        var result = _repository.GetStadium(updatedStadium);
        Assert.AreEqual("OtraCiudad", result?._city);
    }
    
    [TestMethod]
    public void GivenNonExistingStadium_UpdateStadium_ShouldThrowException()
    {
        var nonExisting = new Stadium("Centenario", "Montevideo", "Desc", 60000);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.UpdateStadium(nonExisting));
    }
    
    [TestMethod]
    public void GivenExistingStadium_UpdateStadium_WithDuplicateName_ShouldThrowException()
    {
        _service.AddStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium("Estadio Centenario", "Montevideo", "Estadio historico", 60000);

        var updated = new Stadium("Estadio Centenario", "OtraCiudad", "OtraDesc", 40600);

        Assert.ThrowsException<ArgumentException>(() => _service.UpdateStadium(updated));
    }
}