using Domain;

namespace DomainTests;

[TestClass]
public class LogEntryTests
{
    [TestMethod]
    public void Constructor_WithValidData_ShouldCreateLogEntry()
    {
        var userEmail = "admin@test.com";
        var category = "Usuarios";
        var action = "Alta";
        var detail = "Se creó el usuario usuario@test.com";

        var logEntry = new LogEntry(userEmail, category, action, detail);

        Assert.AreEqual(userEmail, logEntry.UserEmail);
        Assert.AreEqual(category, logEntry.Category);
        Assert.AreEqual(action, logEntry.Action);
        Assert.AreEqual(detail, logEntry.Detail);
        Assert.IsTrue(logEntry.Timestamp <= DateTime.UtcNow);
    }
    
    [TestMethod]
    public void Constructor_WithEmptyCategory_ShouldThrowException()
    {
        var userEmail = "admin@test.com";
        var action = "Alta";
        var detail = "Se creó el usuario usuario@test.com";

        Assert.ThrowsException<ArgumentException>(() =>
            new LogEntry(userEmail, string.Empty, action, detail));
    }
    
    [TestMethod]
    public void Constructor_WithEmptyAction_ShouldThrowException()
    {
        var userEmail = "admin@test.com";
        var category = "Usuarios";
        var detail = "Se creó el usuario usuario@test.com";

        Assert.ThrowsException<ArgumentException>(() =>
            new LogEntry(userEmail, category, string.Empty, detail));
    }
}