using Domain.Entities;
using IRepository;

namespace Repository;

public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly List<Notification> _notifications = new();

    public void Add(Notification notification)
    {
        _notifications.Add(notification);
    }

    public Notification? GetById(Guid id)
    {
        return _notifications.FirstOrDefault(notification => notification.Id == id);
    }
}