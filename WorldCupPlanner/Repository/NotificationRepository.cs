using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

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
            .FirstOrDefault(n => EF.Property<Guid>(n, "_id") == id);
    }

    public List<Notification> GetByRecipient(string email)
    {
        return _context.Notifications
            .Where(n => EF.Property<string>(n, "_recipientEmail") == email)
            .ToList();
    }

    public List<Notification> GetUnread(string email)
    {
        return _context.Notifications
            .Where(n => EF.Property<string>(n, "_recipientEmail") == email &&
                        !EF.Property<bool>(n, "_isRead"))
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