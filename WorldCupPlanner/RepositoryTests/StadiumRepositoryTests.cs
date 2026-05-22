using Domain.Entities;
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
    
    [TestMethod]
    public void Add_OneStadium_ShouldNotThrowException()
    {
        _stadiumRepository.AddStadium(_stadium);
        var result = _stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Add_NullStadium_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _stadiumRepository.AddStadium(null!));
    }
    
    [TestMethod]
    public void Get_ExistingStadium_ShouldReturnStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        var result = _stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNotNull(result);
        Assert.AreEqual("Campeon del Siglo", result.StadiumName);
    }
    
    [TestMethod]
    public void Get_NonExistingStadium_ShouldReturnNull()
    {
        var result = _stadiumRepository.GetStadium("Centenario");
        Assert.IsNull(result);
    }
    
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

    [TestMethod]
    public void GetAll_ModifyingReturnedList_ShouldNotAffectRepository()
    {
        _stadiumRepository.AddStadium(_stadium);
        var list = _stadiumRepository.GetAllStadiums();
        list.Clear();
        var result = _stadiumRepository.GetAllStadiums();
        Assert.AreEqual(1, result.Count);
    }
    
    [TestMethod]
    public void GivenExistingStadium_DeleteStadium_ShouldDeleteStadium()
    {
        _stadiumRepository.AddStadium(_stadium);
        _stadiumRepository.DeleteStadium(_stadium);
        var result = _stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void Delete_NullStadium_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _stadiumRepository.DeleteStadium(null!)
        );
    }
}