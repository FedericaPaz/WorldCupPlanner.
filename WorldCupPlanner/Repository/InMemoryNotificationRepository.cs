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
    
    public IEnumerable<Notification> GetUnreadByUserEmail(string userEmail)
    {
        return _notifications
            .Where(notification =>
                notification.UserEmail == userEmail &&
                !notification.IsRead)
            .ToList();
    }
    
    public void Update(Notification notification)
    {
        var existingNotification = GetById(notification.Id);

        if (existingNotification is null)
        {
            throw new InvalidOperationException("La notificación no existe.");
        }
    }
}
