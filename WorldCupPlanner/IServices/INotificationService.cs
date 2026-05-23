using Domain.Entities;

namespace IServices;

public interface INotificationService
{
    void Send(string recipientEmail, string message);
    void NotifyJournalists(string message);
    IEnumerable<Notification> GetUnread(string email);
    void MarkAsRead(Guid notificationId, string userEmail);
}