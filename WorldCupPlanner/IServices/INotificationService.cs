using Domain.Entities;

namespace IServices;

public interface INotificationService
{
    void NotifyJournalists(string message);
    IEnumerable<Notification> GetUnreadNotifications(string userEmail);
}