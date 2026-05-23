using Domain.Entities;

namespace Repository;

public interface INotificationRepository
{
    void Add(Notification notification);
    List<Notification> GetByRecipient(string email);
    List<Notification> GetUnread(string email);
}