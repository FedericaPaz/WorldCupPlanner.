using Domain.Entities;
using DTOs.Stadium;
using DomainTests.Stubs;
using IRepository;
using Services;
using Services.Mappers;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class StadiumServiceTests
{
    private StadiumService _service;
    private IStadiumRepository _repository = null!;
    private IMatchRepository _matchrepository = new MatchRepositoryStub();

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
        _repository = new StadiumRepositoryStub();
        _service = new StadiumService(_repository,_matchrepository);
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
        var dto = CreateRequest("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium(dto);
        Assert.ThrowsException<InvalidOperationException>(() => _service.AddStadium(dto));
    }

    [TestMethod]
    public void AddNullRequest_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.AddStadium(null!));
    }
    
    [TestMethod]
    public void AddStadium_WithNullStadium_ShouldThrowException()
    {
        var dto = CreateRequest("", "Montevideo", "Estadio de Peniarol", 40700);
        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(dto));
    }
    
    [TestMethod]
    public void AddStadium_WithNullCity_ShouldThrowException()
    {
        var dto = CreateRequest("Campeon del Siglo", "", "Estadio de Peniarol", 40700);
        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(dto));
    }

    [TestMethod]
    public void AddStadium_WithTooLongCity_ShouldThrowException()
    {
        var dto = CreateRequest("Camepon del siglo", new string('a', 61), "Estadio de Peniarol", 40700);
        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(dto));
    }

    [TestMethod]
    public void AddStadium_WithTooLongDescription_ShouldThrowException()
    {
        var dto = CreateRequest("Campeon del Siglo", "Montevideo", new string('a', 401), 40700);
        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(dto));
    }

    [TestMethod]
    public void AddStadium_WithCapacityBelowMinimum_ShouldThrowException()
    {
        var dto = CreateRequest("Camepon del Siglo", "Montevideo", "Estadio de Peniarol", 10000);
        Assert.ThrowsException<ArgumentException>(() => _service.AddStadium(dto));
    }

    [TestMethod]
    public void DeleteStadium_WithExistingStadium_ShouldNotThrowException()
    {
        var dto = CreateRequest("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium(dto);
        _service.DeleteStadium(dto.stadiumName);
        var result = _service.GetStadium(dto.stadiumName);
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void DeleteStadium_WithNonExistingStadium_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _service.DeleteStadium("El estadio no existe"));
    }

    [TestMethod]
    public void DeleteStadium_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => _service.DeleteStadium(string.Empty));
    }
    
    [TestMethod]
    public void GetAllStadiums_WithNoStadiums_ShouldReturnEmptyList()
    {
        var result = _service.GetAllStadiums();
        Assert.AreEqual(0, result.Count);
    }

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
    public void GetStadium_WithExistingStadium_ShouldNotThrowException()
    {
        var dto = CreateRequest("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium(dto);
        var result = _service.GetStadium(dto.stadiumName);
        Assert.IsNotNull(result);
        Assert.AreEqual(dto.stadiumCity, result.stadiumCity);
    }
   
    [TestMethod]
    public void GetStadium_WithNonExistingStadium_ShouldReturnNull()
    {
        var result = _service.GetStadium("Campeon del Siglo");
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetStadium_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => _service.GetStadium(string.Empty));
    }

    [TestMethod]
    public void DeleteStadium_WhenStadiumHasAssignedMatches_ShouldThrowException()
    {
        var dto = CreateRequest("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        _service.AddStadium(dto);

        var clock = new DateTimeMatchStub(new DateTime(2025, 1, 1));
        var stadium = dto.ToEntity();
        var homeTeam = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);
        var awayTeam = new Team("Argentina", new Confederation("CONMEBOL"), 1700);
        var match = new Match(1, new DateTime(2026, 6, 1), stadium, homeTeam, awayTeam, 'A', clock);
        _matchrepository.AddMatch(match);

        Assert.ThrowsException<InvalidOperationException>(() => _service.DeleteStadium(dto.stadiumName));
    }

    [TestMethod]
    public void GetAllStadiums_WithStadiums_ShouldReturnAll()
    {
        _service.AddStadium(CreateRequest("Estadio A", "Ciudad A", null, 40000));
        _service.AddStadium(CreateRequest("Estadio B", "Ciudad B", null, 50000));

        var result = _service.GetAllStadiums();

        Assert.AreEqual(2, result.Count);
    }
}