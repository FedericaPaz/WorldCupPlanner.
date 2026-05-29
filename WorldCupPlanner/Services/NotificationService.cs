using Domain.Entities;
using Domain.Enums;
using IRepository;
using IServices;

namespace Services;

public class NotificationService : INotificationService
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogService _logService;

    public NotificationService(
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        ILogService logService)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _logService = logService;
    }

    public void NotifyJournalists(string message)
    {
        var users = _userRepository.GetAllUsers();

        foreach (var user in users)
        {
            if (user.Roles.Contains(UserRole.Journalist))
            {
                Send(user.Email, message);
            }
        }
    }
    
    public IEnumerable<Notification> GetUnread(string email)
    {
        return _notificationRepository.GetUnreadByUserEmail(email);
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
        
        _logService.Register(userEmail, "Notification", "NotificationRead", $"El usuario {userEmail} marcó como leída la notificación {notification.Id}.");
    }
    
    public void Send(string recipientEmail, string message)
    {
        var notification = new Notification(
            recipientEmail,
            message,
            DateTime.UtcNow);

        _notificationRepository.Add(notification);
    }
}