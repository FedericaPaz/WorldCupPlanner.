using Domain.Entities;
using IServices;
namespace ServicesTests.Stubs;

public class NotificationServiceStub : INotificationService
{
    public string? LastJournalistMessage { get; private set; }

    public void Send(string recipientEmail, string message)
    {
        // no-op for tests
    }

    public void NotifyJournalists(string message)
    {
        LastJournalistMessage = message;
    }

    public IEnumerable<Notification> GetUnread(string email)
    {
        return Enumerable.Empty<Notification>();
    }

    public void MarkAsRead(Guid notificationId, string userEmail)
    {
        // no-op for tests
    }
}