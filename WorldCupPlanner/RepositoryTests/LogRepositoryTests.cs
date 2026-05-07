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
            "admin@test.com",
            "Usuarios",
            "Alta",
            "Se creó el usuario usuario@test.com"
        );

        repository.Add(logEntry);

        var logs = repository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual(logEntry, logs[0]);
    }
}