using Domain.Entities;
using Domain.Enums;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class NotificationServiceTests
{
    [TestMethod]
    public void NotifyJournalists_WhenUsersExist_ShouldCreateNotificationOnlyForJournalists()
    {
        var userRepository = new UserRepository();
        var notificationRepository = new InMemoryNotificationRepository();

        var journalist = new User("Juan", "Periodista", "periodista@test.com", new DateTime(1995, 1, 1), "Aa123456!", new List<UserRole> { UserRole.Journalist });

        var editor = new User("Pedro", "Editor", "editor@test.com", new DateTime(1995, 1, 1), "Aa123456!", new List<UserRole> { UserRole.Editor });

        userRepository.AddUser(journalist);
        userRepository.AddUser(editor);

        var service = new NotificationService(userRepository, notificationRepository);

        service.NotifyJournalists("Se cargó un nuevo resultado");

        var journalistNotifications = notificationRepository
            .GetUnreadByUserEmail("periodista@test.com")
            .ToList();

        var editorNotifications = notificationRepository
            .GetUnreadByUserEmail("editor@test.com")
            .ToList();

        Assert.AreEqual(1, journalistNotifications.Count);
        Assert.AreEqual("Se cargó un nuevo resultado", journalistNotifications[0].Message);
        Assert.AreEqual(0, editorNotifications.Count);
    }
    
    [TestMethod]
    public void GetUnreadNotifications_WhenUserHasUnreadNotifications_ShouldReturnUnreadNotifications()
    {
        var userRepository = new UserRepository();
        var notificationRepository = new InMemoryNotificationRepository();

        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado", new DateTime(2026, 6, 1, 14, 30, 0));

        notificationRepository.Add(notification);

        var service = new NotificationService(userRepository, notificationRepository);

        var unreadNotifications = service
            .GetUnread("periodista@test.com")
            .ToList();

        Assert.AreEqual(1, unreadNotifications.Count);
        Assert.AreSame(notification, unreadNotifications[0]);
    }
    
    [TestMethod]
    public void MarkAsRead_WithExistingNotification_ShouldMarkNotificationAsRead()
    {
        var userRepository = new UserRepository();
        var notificationRepository = new InMemoryNotificationRepository();

        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado", new DateTime(2026, 6, 1, 14, 30, 0));

        notificationRepository.Add(notification);

        var service = new NotificationService(userRepository, notificationRepository);

        service.MarkAsRead(notification.Id, "periodista@test.com");

        var storedNotification = notificationRepository.GetById(notification.Id);

        Assert.IsNotNull(storedNotification);
        Assert.IsTrue(storedNotification.IsRead);
    }
    
    [TestMethod]
    public void MarkAsRead_WhenNotificationBelongsToAnotherUser_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var notificationRepository = new InMemoryNotificationRepository();

        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado", new DateTime(2026, 6, 1, 14, 30, 0));

        notificationRepository.Add(notification);

        var service = new NotificationService(userRepository, notificationRepository);

        Assert.ThrowsException<InvalidOperationException>(() =>
            service.MarkAsRead(notification.Id, "otro@test.com"));
    }
    
    [TestMethod]
    public void Send_WithValidRecipientAndMessage_ShouldCreateUnreadNotification()
    {
        var userRepository = new UserRepository();
        var notificationRepository = new InMemoryNotificationRepository();
        var service = new NotificationService(userRepository, notificationRepository);

        service.Send("periodista@test.com", "Nueva notificación");

        var notifications = notificationRepository
            .GetUnreadByUserEmail("periodista@test.com")
            .ToList();

        Assert.AreEqual(1, notifications.Count);
        Assert.AreEqual("Nueva notificación", notifications[0].Message);
    }
    
    [TestMethod]
    public void GetUnread_WhenUserHasUnreadNotifications_ShouldReturnUnreadNotifications()
    {
        var userRepository = new UserRepository();
        var notificationRepository = new InMemoryNotificationRepository();

        var notification = new Notification(
            "periodista@test.com",
            "Nueva notificación",
            new DateTime(2026, 6, 1, 14, 30, 0));

        notificationRepository.Add(notification);

        var service = new NotificationService(userRepository, notificationRepository);

        var notifications = service.GetUnread("periodista@test.com").ToList();

        Assert.AreEqual(1, notifications.Count);
        Assert.AreSame(notification, notifications[0]);
    }
}