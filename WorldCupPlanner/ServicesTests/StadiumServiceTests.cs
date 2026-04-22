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
    public void CreateStadium_WithValidData_ShouldNotThrowException()
    {
        _service.CreateStadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
    }
}