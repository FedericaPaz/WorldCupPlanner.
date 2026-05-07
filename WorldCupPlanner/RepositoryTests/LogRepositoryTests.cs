using Domain;
using Repository;

namespace RepositoryTests;

[TestClass]
public class LogRepositoryTests
{
    [TestMethod]
    public void Add_WithValidLogEntry_ShouldStoreLogEntry()
    {
        var repository = new LogRepository();
        var logEntry = new LogEntry(
            "admin@test.com", "Usuarios","Alta","Se creó el usuario usuario@test.com"
        );
        repository.Add(logEntry);
        
        var logs = repository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual(logEntry, logs[0]);
    }
    
    [TestMethod]
    public void Add_WithNullLogEntry_ShouldThrowException()
    {
        var repository = new LogRepository();
        Assert.ThrowsException<ArgumentNullException>(() => repository.Add(null!));
    }
    
    [TestMethod]
    public void GetAll_WithMultipleLogs_ShouldReturnLogsOrderedByNewestFirst()
    {
        var repository = new LogRepository();

        var firstLog = new LogEntry("admin@test.com", "Usuarios", "Alta", "Se creó el usuario primero@test.com"
        );

        Thread.Sleep(10);

        var secondLog = new LogEntry("admin@test.com", "Usuarios", "Alta", "Se creó el usuario segundo@test.com"
        );

        repository.Add(firstLog);
        repository.Add(secondLog);

        var logs = repository.GetAll();

        Assert.AreEqual(secondLog, logs[0]);
        Assert.AreEqual(firstLog, logs[1]);
    }
}