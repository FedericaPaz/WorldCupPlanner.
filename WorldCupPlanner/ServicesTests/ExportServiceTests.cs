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
    
    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsJournalist_ShouldReturnBytes()
    {
        var userRepository = new UserRepository();

        var journalist = new User("Juan", "Periodista", "periodista@test.com", new DateTime(1995, 1, 1), "Aa123456!", new List<UserRole> { UserRole.Journalist });

        userRepository.AddUser(journalist);

        var service = new ExportService(userRepository);
        var result = service.ExportFixtureToCsv("periodista@test.com");

        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsEditor_ShouldThrowUnauthorizedAccessException()
    {
        var userRepository = new UserRepository();

        var editor = new User("Pedro", "Editor", "editor@test.com", new DateTime(1995, 1, 1), "Aa123456!", new List<UserRole> { UserRole.Editor });

        userRepository.AddUser(editor);

        var service = new ExportService(userRepository);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            service.ExportFixtureToCsv("editor@test.com"));
    }
    
    [TestMethod]
    public void ExportFixtureToXlsx_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        var userRepository = new UserRepository();

        var administrator = new User("Admin", "Sistema", "admin@test.com", new DateTime(1990, 1, 1), "Aa123456!", new List<UserRole> { UserRole.SystemAdministrator });

        userRepository.AddUser(administrator);

        var service = new ExportService(userRepository);

        var result = service.ExportFixtureToXlsx("admin@test.com");

        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void ExportLogsToCsv_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        var userRepository = new UserRepository();

        var administrator = new User("Admin", "Sistema", "admin@test.com", new DateTime(1990, 1, 1), "Aa123456!", new List<UserRole> { UserRole.SystemAdministrator });

        userRepository.AddUser(administrator);

        var service = new ExportService(userRepository);

        var from = new DateTime(2026, 6, 1);
        var to = new DateTime(2026, 6, 10);

        var result = service.ExportLogsToCsv("admin@test.com", from, to);

        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void ExportLogsToXlsx_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        var userRepository = new UserRepository();

        var administrator = new User("Admin", "Sistema", "admin@test.com", new DateTime(1990, 1, 1), "Aa123456!", new List<UserRole> { UserRole.SystemAdministrator });

        userRepository.AddUser(administrator);

        var service = new ExportService(userRepository);

        var from = new DateTime(2026, 6, 1);
        var to = new DateTime(2026, 6, 10);

        var result = service.ExportLogsToXlsx("admin@test.com", from, to);

        Assert.IsNotNull(result);
    }
}