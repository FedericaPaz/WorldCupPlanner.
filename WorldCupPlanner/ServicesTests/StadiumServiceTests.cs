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
}