namespace Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public string UserEmail { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public Notification(string userEmail, string message, DateTime createdAt)
    {
        Id = Guid.NewGuid();
        UserEmail = userEmail;
        Message = message;
        CreatedAt = createdAt;
        IsRead = false;
        ReadAt = null;
    }

    public void MarkAsRead(DateTime readAt)
    {
        IsRead = true;
        ReadAt = readAt;
    }
    
    public void MarkAsRead()
    {
        MarkAsRead(DateTime.UtcNow);
    }
}