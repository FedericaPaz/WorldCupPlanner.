using Domain.Entities;
using IRepository;

namespace Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly List<Notification> _notifications = new();

    public void Add(Notification notification)
    {
        _notifications.Add(notification);
    }
    public List<Notification> GetByRecipient(string email)
    {
        return _notifications
            .Where(notification => notification.RecipientEmail == email)
            .ToList();
    }

    public List<Notification> GetUnread(string email)
    {
        return _notifications
            .Where(notification =>
                notification.RecipientEmail == email &&
                !notification.IsRead)
            .ToList();
    }
}