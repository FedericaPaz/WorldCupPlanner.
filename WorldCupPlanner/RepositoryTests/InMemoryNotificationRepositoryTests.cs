using Domain.Entities;
using Repository;

namespace RepositoryTests;

[TestClass]
public class InMemoryNotificationRepositoryTests
{
    [TestMethod]
    public void Add_WithValidNotification_ShouldStoreNotification()
    {
        var repository = new InMemoryNotificationRepository();
        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado", new DateTime(2026, 6, 1, 14, 30, 0));
        repository.Add(notification);
        var storedNotification = repository.GetById(notification.Id);
        Assert.AreSame(notification, storedNotification);
    }
    
    [TestMethod]
    public void GetUnreadByUserEmail_WhenUserHasReadAndUnreadNotifications_ShouldReturnOnlyUnread()
    {
        var repository = new InMemoryNotificationRepository();

        var unreadNotification = new Notification(
            "periodista@test.com",
            "Resultado nuevo",
            new DateTime(2026, 6, 1, 14, 30, 0));

        var readNotification = new Notification(
            "periodista@test.com",
            "Resultado viejo",
            new DateTime(2026, 6, 1, 13, 30, 0));

        readNotification.MarkAsRead(new DateTime(2026, 6, 1, 15, 0, 0));

        repository.Add(unreadNotification);
        repository.Add(readNotification);

        var unreadNotifications = repository.GetUnreadByUserEmail("periodista@test.com").ToList();

        Assert.AreEqual(1, unreadNotifications.Count);
        Assert.AreSame(unreadNotification, unreadNotifications[0]);
    }
    
    [TestMethod]
    public void Update_WithExistingNotification_ShouldKeepUpdatedNotification()
    {
        var repository = new InMemoryNotificationRepository();

        var notification = new Notification(
            "periodista@test.com",
            "Resultado nuevo",
            new DateTime(2026, 6, 1, 14, 30, 0));

        repository.Add(notification);

        var readAt = new DateTime(2026, 6, 1, 15, 0, 0);
        notification.MarkAsRead(readAt);

        repository.Update(notification);

        var storedNotification = repository.GetById(notification.Id);

        Assert.IsNotNull(storedNotification);
        Assert.IsTrue(storedNotification.IsRead);
        Assert.AreEqual(readAt, storedNotification.ReadAt);
    }
}