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
}