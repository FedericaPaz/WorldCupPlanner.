using Domain.Entities;
using Repository;
using Microsoft.EntityFrameworkCore;

namespace RepositoryTests;

[TestClass]
public class NotificationRepositoryTests
{
    private WorldCupPlannerContext _context = null!;
    private NotificationRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);
        _repository = new NotificationRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
    
    [TestMethod]
    public void AddNotification_WithValidNotification_ShouldStoreNotification()
    {
        var notification = new Notification(
            "periodista@gmail.com",
            "Nuevo resultado disponible"
        );

        _repository.Add(notification);

        var result = _repository.GetByRecipient("periodista@gmail.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(notification, result[0]);
    }
    
    [TestMethod]
    public void GetById_WithExistingNotification_ShouldReturnNotification()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Se cargó un nuevo resultado",
            new DateTime(2026, 6, 1, 14, 30, 0));

        _repository.Add(notification);

        var storedNotification = _repository.GetById(notification.Id);

        Assert.AreEqual(notification.Id, storedNotification!.Id);
    }
    
    [TestMethod]
    public void GetByRecipient_WithStoredNotification_ShouldReturnNotification()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        _repository.Add(notification);

        var result = _repository.GetByRecipient("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(notification.Id, result[0].Id);
    }
    
    [TestMethod]
    public void GetByRecipient_WithNoNotifications_ShouldReturnEmptyList()
    {
        var result = _repository.GetByRecipient("periodista@test.com");

        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void GetByRecipient_WithDifferentRecipients_ShouldReturnOnlyMatchingRecipient()
    {
        var journalistNotification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        var adminNotification = new Notification(
            "admin@test.com",
            "Notificación para admin"
        );

        _repository.Add(journalistNotification);
        _repository.Add(adminNotification);

        var result = _repository.GetByRecipient("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(journalistNotification.Id, result[0].Id);
    }

    [TestMethod]
    public void GetUnreadNotification_WithUnreadNotification_ShouldReturnNotification()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        _repository.Add(notification);

        var result = _repository.GetUnread("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(notification.Id, result[0].Id);
    }

    [TestMethod]
    public void GetUnread_WithReadNotification_ShouldNotReturnNotification()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        notification.MarkAsRead();

        _repository.Add(notification);

        var result = _repository.GetUnread("periodista@test.com");

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetUnread_WithNotificationsFromDifferentRecipients_ShouldReturnOnlyUnreadNotificationsForRecipient()
    {
        var journalistNotification = new Notification(
            "periodista@test.com",
            "Nuevo resultado disponible"
        );

        var adminNotification = new Notification(
            "admin@test.com",
            "Notificación para admin"
        );

        _repository.Add(journalistNotification);
        _repository.Add(adminNotification);

        var result = _repository.GetUnread("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(journalistNotification.Id, result[0].Id);
    }

    [TestMethod]
    public void GetUnread_WithReadAndUnreadNotificationsForSameRecipient_ShouldReturnOnlyUnreadNotifications()
    {
        var unreadNotification = new Notification(
            "periodista@test.com",
            "Notificación no leída"
        );

        var readNotification = new Notification(
            "periodista@test.com",
            "Notificación leída"
        );

        readNotification.MarkAsRead();

        _repository.Add(unreadNotification);
        _repository.Add(readNotification);

        var result = _repository.GetUnread("periodista@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(unreadNotification.Id, result[0].Id);
    }

    [TestMethod]
    public void GetUnreadByUserEmail_WhenUserHasReadAndUnreadNotifications_ShouldReturnOnlyUnread()
    {
        var unreadNotification = new Notification(
            "periodista@test.com",
            "Resultado nuevo",
            new DateTime(2026, 6, 1, 14, 30, 0));

        var readNotification = new Notification(
            "periodista@test.com",
            "Resultado viejo",
            new DateTime(2026, 6, 1, 13, 30, 0));

        readNotification.MarkAsRead(new DateTime(2026, 6, 1, 15, 0, 0));

        _repository.Add(unreadNotification);
        _repository.Add(readNotification);

        var unreadNotifications = _repository
            .GetUnreadByUserEmail("periodista@test.com")
            .ToList();

        Assert.AreEqual(1, unreadNotifications.Count);
        Assert.AreEqual(unreadNotification.Id, unreadNotifications[0].Id);
    }

    [TestMethod]
    public void Update_WithExistingNotification_ShouldKeepUpdatedNotification()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Resultado nuevo",
            new DateTime(2026, 6, 1, 14, 30, 0));

        _repository.Add(notification);

        var readAt = new DateTime(2026, 6, 1, 15, 0, 0);
        notification.MarkAsRead(readAt);

        _repository.Update(notification);
        var storedNotification = _repository.GetById(notification.Id);
        
        Assert.IsNotNull(storedNotification);
        Assert.IsTrue(storedNotification.IsRead);
        Assert.AreEqual(readAt, storedNotification.ReadAt);
    }
    
    [TestMethod]
    public void Update_WithNonExistingNotification_ShouldThrowInvalidOperationException()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Resultado nuevo",
            new DateTime(2026, 6, 1, 14, 30, 0));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _repository.Update(notification));
    }
}