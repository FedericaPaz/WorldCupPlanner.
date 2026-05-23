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
}