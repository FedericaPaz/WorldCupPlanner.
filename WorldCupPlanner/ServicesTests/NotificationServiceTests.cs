using Domain.Entities;
using Domain.Enums;
using Repository;
using Services;
using ServicesTests.TestHelpers;

namespace ServicesTests;

[TestClass]
public class NotificationServiceTests
{
    private WorldCupPlannerContext _context = null!;
    private UserRepository _userRepository = null!;
    private InMemoryNotificationRepository _notificationRepository = null!;
    private NotificationService _service = null!;
    private LogRepository _logRepository = null!;
    private LogService _logService = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _context = DbContextFactory.CreateInMemoryContext();
        _userRepository = new UserRepository(_context);
        _notificationRepository = new InMemoryNotificationRepository();
        _logRepository = new LogRepository();
        _logService = new LogService(_logRepository);

        _service = new NotificationService(_userRepository, _notificationRepository, _logService);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void NotifyJournalists_WhenUsersExist_ShouldCreateNotificationOnlyForJournalists()
    {
        _userRepository.AddUser(CreateUser("periodista@test.com", UserRole.Journalist));
        _userRepository.AddUser(CreateUser("editor@test.com", UserRole.Editor));

        _service.NotifyJournalists("Se cargó un nuevo resultado");

        var journalistNotifications = _notificationRepository
            .GetUnreadByUserEmail("periodista@test.com").ToList();
        var editorNotifications = _notificationRepository
            .GetUnreadByUserEmail("editor@test.com").ToList();

        Assert.AreEqual(1, journalistNotifications.Count);
        Assert.AreEqual("Se cargó un nuevo resultado", journalistNotifications[0].Message);
        Assert.AreEqual(0, editorNotifications.Count);
    }

    [TestMethod]
    public void GetUnreadNotifications_WhenUserHasUnreadNotifications_ShouldReturnUnreadNotifications()
    {
        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado",
            new DateTime(2026, 6, 1, 14, 30, 0));
        _notificationRepository.Add(notification);

        var unreadNotifications = _service.GetUnread("periodista@test.com").ToList();

        Assert.AreEqual(1, unreadNotifications.Count);
        Assert.AreSame(notification, unreadNotifications[0]);
    }

    [TestMethod]
    public void MarkAsRead_WithExistingNotification_ShouldMarkNotificationAsRead()
    {
        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado",
            new DateTime(2026, 6, 1, 14, 30, 0));
        _notificationRepository.Add(notification);

        _service.MarkAsRead(notification.Id, "periodista@test.com");

        var stored = _notificationRepository.GetById(notification.Id);
        Assert.IsNotNull(stored);
        Assert.IsTrue(stored.IsRead);
    }

    [TestMethod]
    public void MarkAsRead_WhenNotificationBelongsToAnotherUser_ShouldThrowException()
    {
        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado",
            new DateTime(2026, 6, 1, 14, 30, 0));
        _notificationRepository.Add(notification);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.MarkAsRead(notification.Id, "otro@test.com"));
    }

    [TestMethod]
    public void Send_WithValidRecipientAndMessage_ShouldCreateUnreadNotification()
    {
        _service.Send("periodista@test.com", "Nueva notificación");

        var notifications = _notificationRepository
            .GetUnreadByUserEmail("periodista@test.com").ToList();

        Assert.AreEqual(1, notifications.Count);
        Assert.AreEqual("Nueva notificación", notifications[0].Message);
    }

    [TestMethod]
    public void GetUnread_WhenUserHasUnreadNotifications_ShouldReturnUnreadNotifications()
    {
        var notification = new Notification("periodista@test.com", "Nueva notificación",
            new DateTime(2026, 6, 1, 14, 30, 0));
        _notificationRepository.Add(notification);

        var notifications = _service.GetUnread("periodista@test.com").ToList();

        Assert.AreEqual(1, notifications.Count);
        Assert.AreSame(notification, notifications[0]);
    }

    private static User CreateUser(string email, UserRole role)
    {
        string firstName = "Juan";
        string lastName = "Apellido";
        DateTime birthDate = new DateTime(1995, 1, 1);
        string password = "Aa123456!";
        var roles = new List<UserRole>();
        roles.Add(role);

        return new User(firstName, lastName, email, birthDate, password, roles);
    }
    
    [TestMethod]
    public void MarkAsRead_WithExistingNotification_ShouldRegisterNotificationReadLog()
    {
        var notification = new Notification("periodista@test.com", "Se cargó un nuevo resultado", new DateTime(2026, 6, 1, 14, 30, 0));

        _notificationRepository.Add(notification);

        _service.MarkAsRead(notification.Id, "periodista@test.com");

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("periodista@test.com", logs[0].UserEmail);
        Assert.AreEqual("Notification", logs[0].Category);
        Assert.AreEqual("NotificationRead", logs[0].Action);
    }
    
    
}