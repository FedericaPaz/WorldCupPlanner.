using Domain.Entities;
using Repository;

namespace RepositoryTests;

[TestClass]
public class NotificationRepositoryTests
{
    [TestMethod]
    public void AddNotification_WithValidNotification_ShouldNotThrowException()
    {
        var repository = new NotificationRepository();
        var notification = new Notification(
            "periodista@gmail.com",
            "Nuevo resultado disponible"
        );

        repository.Add(notification);
    }
    
    [TestMethod]
    public void GetByRecipient_WithStoredNotification_ShouldReturnNotification()
    {
        var repository = new NotificationRepository();

        var notification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        repository.Add(notification);

        var result = repository.GetByRecipient("periodista@test.com");

        Assert.AreEqual(1, result.Count);
    }
    
    [TestMethod]
    public void GetByRecipient_WithNoNotifications_ShouldReturnEmptyList()
    {
        var repository = new NotificationRepository();

        var result = repository.GetByRecipient("periodista@test.com");

        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void GetUnreadNotification_WithUnreadNotification_ShouldReturnNotification()
    {
        var repository = new NotificationRepository();

        var notification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        repository.Add(notification);

        var result = repository.GetUnread("periodista@test.com");

        Assert.AreEqual(1, result.Count);
    }
}