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
}