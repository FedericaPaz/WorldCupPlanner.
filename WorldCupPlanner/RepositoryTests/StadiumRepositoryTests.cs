using Domain;
using Repository;

namespace RepositoryTests;

[TestClass]
public class StadiumRepositoryTests
{
    [TestMethod]
    public void Add_OneStadium_ShouldNotThrowException()
    {
        var stadiumRepository = new StadiumRepository();
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        stadiumRepository.AddStadium(stadium);
    }

    [TestMethod]
    public void Get_ExistingStadium_ShouldReturnStadium()
    {
        var stadiumRepository = new StadiumRepository();
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        stadiumRepository.AddStadium(stadium);
        var result = stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GivenNullStadium_AddStadium_ShouldThrowException()
    {
        var stadiumRepository = new StadiumRepository();
        Assert.ThrowsException<ArgumentNullException>(() => stadiumRepository.AddStadium(null));
    }
    
    [TestMethod]
    public void GivenExistingStadium_AddDuplicate_ShouldThrowException()
    {
        var stadiumRepository = new StadiumRepository();
        var stadium1 = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        var stadium2 = new Stadium("Campeon del Siglo", "OtraCiudad", "OtraDesc", 40600);
        stadiumRepository.AddStadium(stadium1);
        Assert.ThrowsException<ArgumentException>(() => stadiumRepository.AddStadium(stadium2));
    }

    [TestMethod]
    public void GivenExistingStadium_UpdateStadium_ShouldUpdateStadium()
    {
        var stadiumRepository = new StadiumRepository();
        var originalStadium = new Stadium("Campeon del Siglo", "Montevideo", "Estadio de Peniarol", 40700);
        stadiumRepository.AddStadium(originalStadium);
        var updatedStadium = new Stadium("Campeon del Siglo",  "otraCiudad", "otraDesc", 40600);
        stadiumRepository.UpdateStadium(updatedStadium);
        var result = stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.AreEqual("otraDesc", result?._description);
    }

    [TestMethod]
    public void GivenNonExistingStadium_DeleteStadium_ShouldThrowException()
    {
        var stadiumRepository = new StadiumRepository();
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Desc", 40000);
        Assert.ThrowsException<InvalidOperationException>(() => stadiumRepository.DeleteStadium(stadium));
    }

    [TestMethod]
    public void GivenNonExistingStadium_UpdateStadium_ShouldThrowException()
    {
        var stadiumRepository = new StadiumRepository();
        var stadium = new Stadium("Campeon del siglo","OtraCiudad", "OtraDesc", 40600);
        Assert.ThrowsException<InvalidOperationException>(() => stadiumRepository.UpdateStadium(stadium));
    }

    [TestMethod]
    public void GivenExistingStadium_DeleteStadium_ShouldDeleteStadium()
    {
        var stadiumRepository = new StadiumRepository();
        var stadium = new Stadium("Campeon del Siglo", "Montevideo", "Desc", 40600);
        stadiumRepository.AddStadium(stadium);
        stadiumRepository.DeleteStadium(stadium);
        var result = stadiumRepository.GetStadium("Campeon del Siglo");
        Assert.IsNull(result);
    }

    
    
}