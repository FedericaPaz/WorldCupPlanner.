using Domain.Entities;

namespace IRepository;

public interface INotificationRepository
{
    void Add(Notification notification);
    List<Notification> GetByRecipient(string email);
    List<Notification> GetUnread(string email);
}