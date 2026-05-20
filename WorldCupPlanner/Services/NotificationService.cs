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
    
    public IEnumerable<Notification> GetUnreadNotifications(string userEmail)
    {
        return _notificationRepository.GetUnreadByUserEmail(userEmail);
    }
    
    public void MarkAsRead(Guid notificationId, string userEmail)
    {
        var notification = _notificationRepository.GetById(notificationId);

        if (notification is null)
        {
            throw new InvalidOperationException("La notificación no existe.");
        }

        if (notification.UserEmail != userEmail)
        {
            throw new InvalidOperationException("La notificación no pertenece al usuario.");
        }

        notification.MarkAsRead(DateTime.UtcNow);
        _notificationRepository.Update(notification);
    }
}