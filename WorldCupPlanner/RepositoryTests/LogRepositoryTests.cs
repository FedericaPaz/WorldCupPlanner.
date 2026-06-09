using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryTests;

[TestClass]
public class LogRepositoryTests
{
    private WorldCupPlannerContext _context = null!;
    private LogRepository _logRepository = null!;
    private LogEntry _log = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new WorldCupPlannerContext(options);
        _logRepository = new LogRepository(_context);
        _log = new LogEntry(
            "admin@test.com", "Usuarios","Alta","Se creó el usuario usuario@test.com"
        );
    }
    
    [TestMethod]
    public void Add_WithNullLogEntry_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _logRepository.Add(null!));
    }
    
    [TestMethod]
    public void GetAll_WithMultipleLogs_ShouldReturnLogsOrderedByNewestFirst()
    {

        var firstLog = new LogEntry("admin@test.com", "Usuarios", "Alta", "Se creó el usuario primero@test.com"
        );

        Thread.Sleep(10);

        var secondLog = new LogEntry("admin@test.com", "Usuarios", "Alta", "Se creó el usuario segundo@test.com"
        );

        _logRepository.Add(firstLog);
        _logRepository.Add(secondLog);

        var logs = _logRepository.GetAll();

        Assert.AreEqual(secondLog, logs[0]);
        Assert.AreEqual(firstLog, logs[1]);
    }
    
    [TestMethod]
        public void Add_WithValidLogEntry_ShouldPersistLogEntry()
        {
            _logRepository.Add(_log);

            var logs = _logRepository.GetAll();

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("admin@test.com", logs[0].UserEmail);
            Assert.AreEqual("Usuarios", logs[0].Category);
            Assert.AreEqual("Alta", logs[0].Action);
            Assert.AreEqual("Se creó el usuario usuario@test.com", logs[0].Detail);
        }
}