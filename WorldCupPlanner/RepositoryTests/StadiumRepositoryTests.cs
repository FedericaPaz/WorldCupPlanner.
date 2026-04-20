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
    
    [TestMethod]
    public void GivenNullStadium_AddStadium_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _stadiumRepository.AddStadium(null));
    }
    
    // Tests para GET

    [TestMethod]
    public void Get_ExistingStadium_ShouldReturnStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        var result = _stadiumRepository.GetStadium(_stadium);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void GetNullStadium_NullStadium_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _stadiumRepository.GetStadium(null));
    }
    
    // Tests GET ALL
    
    [TestMethod]
    public void GivenMultipleStadiums_GetAllStadiums_ShouldReturnStadiums()
    {
        var stadium2 = new Stadium("Estadio Centenario", "Montevideo",  "Desc", 60000);
        _stadiumRepository.AddStadium(_stadium);
        _stadiumRepository.AddStadium(stadium2);
        var result = _stadiumRepository.GetAllStadiums();
        Assert.AreEqual(2, result.Count);
    }
    
    // Test UPDATE
    
    [TestMethod]
    public void GivenExistingStadium_UpdateStadium_ShouldUpdateStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        var updatedStadium = new Stadium("Campeon del Siglo", "OtraCiudad", "OtraDesc", 40600);
        _stadiumRepository.UpdateStadium(updatedStadium);
        var result = _stadiumRepository.GetStadium(updatedStadium);
        Assert.AreEqual("OtraDesc", result?._description);
    }
    
    [TestMethod]
    public void GivenNonExistingStadium_UpdateStadium_ShouldThrowException()
    {
        var stadium = new Stadium("Campeon del Siglo", "OtraCiudad", "OtraDesc", 40600);
        Assert.ThrowsException<InvalidOperationException>(() => _stadiumRepository.UpdateStadium(stadium));
    }
    
    // Tests DELETE 
    
    [TestMethod]
    public void GivenExistingStadium_DeleteStadium_ShouldDeleteStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        _stadiumRepository.DeleteStadium(_stadium);
        var result = _stadiumRepository.GetStadium(_stadium);
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void GivenNonExistingStadium_DeleteStadium_ShouldThrowException()
    {
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Desc", 40000);
        Assert.ThrowsException<InvalidOperationException>(() => _stadiumRepository.DeleteStadium(stadium));
    }
    
}