using Domain.Entities;
using IRepository;

namespace Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly WorldCupPlannerContext _context;

    public NotificationRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void Add(Notification notification)
    {
        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public Notification? GetById(Guid id)
    {
        return _context.Notifications
            .FirstOrDefault(notification => notification.Id == id);
    }

    public List<Notification> GetByRecipient(string email)
    {
        return _context.Notifications
            .Where(notification => notification.RecipientEmail == email)
            .ToList();
    }

    public List<Notification> GetUnread(string email)
    {
        return _context.Notifications
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

        _context.Notifications.Update(notification);
        _context.SaveChanges();
    }
}