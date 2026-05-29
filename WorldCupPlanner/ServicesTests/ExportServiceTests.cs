using Domain.Entities;
using Domain.Enums;
using Repository;
using Services;
using ServicesTests.TestHelpers;

namespace ServicesTests;

[TestClass]
public class ExportServiceTests
{
    private WorldCupPlannerContext _context = null!;
    private UserRepository _userRepository = null!;
    private LogRepository _logRepository = null!;
    private LogService _logService = null!;
    private ExportService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextFactory.CreateInMemoryContext();

        _userRepository = new UserRepository(_context);
        _logRepository = new LogRepository();
        _logService = new LogService(_logRepository);

        _service = new ExportService(_userRepository, _logService);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportFixtureToCsv("admin@test.com");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsJournalist_ShouldReturnBytes()
    {
        AddUser("periodista@test.com", UserRole.Journalist);

        var result = _service.ExportFixtureToCsv("periodista@test.com");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsEditor_ShouldThrowUnauthorizedAccessException()
    {
        AddUser("editor@test.com", UserRole.Editor);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _service.ExportFixtureToCsv("editor@test.com"));
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportFixtureToXlsx("admin@test.com");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportLogsToCsv("admin@test.com", new DateTime(2026, 6, 1), new DateTime(2026, 6, 10));

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ExportLogsToXlsx_WhenUserIsAdministrator_ShouldReturnBytes()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportLogsToXlsx("admin@test.com", new DateTime(2026, 6, 1), new DateTime(2026, 6, 10));

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenFromDateIsAfterToDate_ShouldThrowArgumentException()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ExportLogsToCsv("admin@test.com", new DateTime(2026, 6, 10), new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsAdministrator_ShouldRegisterFileExportedLog()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _service.ExportFixtureToCsv("admin@test.com");

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Export", logs[0].Category);
        Assert.AreEqual("FileExported", logs[0].Action);
    }

    private void AddUser(string email, UserRole role)
    {
        var user = new User("Admin", "Sistema", email, new DateTime(1990, 1, 1), "Aa123456!", new List<UserRole> { role });

        _userRepository.AddUser(user);
    }
}