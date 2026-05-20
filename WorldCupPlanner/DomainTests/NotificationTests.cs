using Domain.Entities;

namespace DomainTests;

[TestClass]
public class NotificationTests
{
    [TestMethod]
    public void CreateNotification_WithValidData_ShouldCreateUnreadNotification()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Se cargó un nuevo resultado",
            new DateTime(2026, 6, 1, 14, 30, 0));

        Assert.AreEqual("periodista@test.com", notification.UserEmail);
        Assert.AreEqual("Se cargó un nuevo resultado", notification.Message);
        Assert.AreEqual(new DateTime(2026, 6, 1, 14, 30, 0), notification.CreatedAt);
        Assert.IsFalse(notification.IsRead);
    }
}