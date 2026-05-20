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
}