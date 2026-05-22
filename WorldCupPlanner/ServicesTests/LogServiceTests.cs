using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class LogServiceTests
{
    [TestMethod]
    public void Register_WithValidData_ShouldCreateLogEntry()
    {
        var repository = new LogRepository();
        var service = new LogService(repository);

        service.Register("admin@test.com", "Usuarios", "Alta", "Se creó el usuario usuario@test.com"
        );

        var logs = service.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Usuarios", logs[0].Category);
        Assert.AreEqual("Alta", logs[0].Action);
        Assert.AreEqual("Se creó el usuario usuario@test.com", logs[0].Detail);
    }
    
    [TestMethod]
    public void Constructor_WithNullRepository_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new LogService(null!));
    }
}