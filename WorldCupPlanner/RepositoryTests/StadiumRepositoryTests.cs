using Domain;
using Repository;

namespace RepositoryTests;

[TestClass]
public class StadiumRepositoryTests
{
    private StadiumRepository _stadiumRepository;
    private Stadium _stadium;

    [TestInitialize]
    public void Setup()
    {
        _stadiumRepository = new StadiumRepository();
        _stadium = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
    }
    
    // Tests para ADD
    [TestMethod]
    public void Add_OneStadium_ShouldNotThrowException()
    {
        _stadiumRepository.AddStadium(_stadium);
    }
    
    // Tests para GET
    [TestMethod]
    public void Get_ExistingStadium_ShouldReturnStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        var result = _stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void Get_NonExistingStadium_ShouldReturnNull()
    {
        var result = _stadiumRepository.GetStadium("Centenario");
        Assert.IsNull(result);
    }
    
    // Tests GET ALL
    [TestMethod]
    public void GetAll_EmptyRepository_ShouldReturnEmptyList()
    {
        var result = _stadiumRepository.GetAllStadiums();
        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void GivenMultipleStadiums_GetAllStadiums_ShouldReturnStadiums()
    {
        var stadium2 = new Stadium("Estadio Centenario", "Montevideo",  "Desc", 60000);
        _stadiumRepository.AddStadium(_stadium);
        _stadiumRepository.AddStadium(stadium2);
        var result = _stadiumRepository.GetAllStadiums();
        Assert.AreEqual(2, result.Count);
    }
    
    // Tests DELETE 
    [TestMethod]
    public void GivenExistingStadium_DeleteStadium_ShouldDeleteStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        _stadiumRepository.DeleteStadium(_stadium);
        var result = _stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNull(result);
    }
}