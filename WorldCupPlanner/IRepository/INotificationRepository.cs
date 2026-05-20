using Domain.Entities;

namespace IRepository;

public interface INotificationRepository
{
    void Add(Notification notification);
    Notification? GetById(Guid id);
    IEnumerable<Notification> GetUnreadByUserEmail(string userEmail);
}

