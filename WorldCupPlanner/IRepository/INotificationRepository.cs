using Domain.Entities;

namespace IRepository;

public interface INotificationRepository
{
    void Add(Notification notification);
    Notification? GetById(Guid id);
    List<Notification> GetByRecipient(string email);
    List<Notification> GetUnread(string email);
    IEnumerable<Notification> GetUnreadByUserEmail(string userEmail);
    void Update(Notification notification);
}