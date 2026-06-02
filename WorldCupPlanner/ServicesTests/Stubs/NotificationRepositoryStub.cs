using Domain.Entities;
using IRepository;

namespace ServicesTests.Stubs;

public class NotificationRepositoryStub : INotificationRepository
{
    private readonly List<Notification> _notifications = new();

    public void Add(Notification notification)
    {
        _notifications.Add(notification);
    }

    public Notification? GetById(Guid id)
    {
        return _notifications
            .FirstOrDefault(notification => notification.Id == id);
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

    public IEnumerable<Notification> GetUnreadByUserEmail(string userEmail)
    {
        return GetUnread(userEmail);
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