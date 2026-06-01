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
        var result = repository.GetByRecipient("periodista@gmail.com");
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(notification, result[0]);
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
    public void GetByRecipient_WithDifferentRecipients_ShouldReturnOnlyMatchingRecipient()
    {
        var repository = new NotificationRepository();

        var journalistNotification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        var adminNotification = new Notification(
            "admin@test.com",
            "Notificación para admin"
        );

        repository.Add(journalistNotification);
        repository.Add(adminNotification);

        var result = repository.GetByRecipient("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(journalistNotification, result[0]);
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
    
    [TestMethod]
    public void GetUnread_WithReadNotification_ShouldNotReturnNotification()
    {
        var repository = new NotificationRepository();

        var notification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        notification.MarkAsRead();

        repository.Add(notification);

        var result = repository.GetUnread("periodista@test.com");

        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void GetUnread_WithNotificationsFromDifferentRecipients_ShouldReturnOnlyUnreadNotificationsForRecipient()
    {
        var repository = new NotificationRepository();

        var journalistNotification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        var adminNotification = new Notification(
            "admin@test.com",
            "Notificación para admin"
        );

        repository.Add(journalistNotification);
        repository.Add(adminNotification);

        var result = repository.GetUnread("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(journalistNotification, result[0]);
    }
    
    [TestMethod]
    public void GetUnread_WithReadAndUnreadNotificationsForSameRecipient_ShouldReturnOnlyUnreadNotifications()
    {
        var repository = new NotificationRepository();

        var unreadNotification = new Notification(
            "periodista@test.com",
            "Notificación no leída"
        );

        var readNotification = new Notification(
            "periodista@test.com",
            "Notificación leída"
        );

        readNotification.MarkAsRead();

        repository.Add(unreadNotification);
        repository.Add(readNotification);

        var result = repository.GetUnread("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(unreadNotification, result[0]);
    }
}