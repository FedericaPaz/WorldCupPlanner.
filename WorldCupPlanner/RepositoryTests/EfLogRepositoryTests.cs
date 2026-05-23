using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryTests;

[TestClass]
public class EfLogRepositoryTests
{
    [TestMethod]
    public void Add_WithValidLogEntry_ShouldPersistLogEntry()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new WorldCupPlannerContext(options);
        var repository = new EfLogRepository(context);

        var logEntry = new LogEntry("admin@test.com", "Test", "Created", "Se creó un registro de prueba");

        repository.Add(logEntry);

        var logs = repository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Test", logs[0].Category);
        Assert.AreEqual("Created", logs[0].Action);
        Assert.AreEqual("Se creó un registro de prueba", logs[0].Detail);
    }
}