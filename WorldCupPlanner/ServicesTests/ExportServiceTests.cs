using Domain.Entities;
using Domain.Enums;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class ExportServiceTests
{
    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        var userRepository = new UserRepository();

        var administrator = new User("Admin", "Sistema", "admin@test.com", new DateTime(1990, 1, 1), "Aa123456!", new List<UserRole> { UserRole.SystemAdministrator });

        userRepository.AddUser(administrator);

        var service = new ExportService(userRepository);

        var result = service.ExportFixtureToCsv("admin@test.com");

        Assert.IsNotNull(result);
    }
}