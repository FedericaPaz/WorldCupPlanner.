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
}