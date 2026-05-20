using Domain.Entities;
using Domain.Enums;
using IRepository;
using IServices;

namespace Services;

public class NotificationService : INotificationService
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public void NotifyJournalists(string message)
    {
        var users = _userRepository.GetAllUsers();

        foreach (var user in users)
        {
            if (user.Roles.Contains(UserRole.Journalist))
            {
                var notification = new Notification(
                    user.Email,
                    message,
                    DateTime.UtcNow);

                _notificationRepository.Add(notification);
            }
        }
    }
}