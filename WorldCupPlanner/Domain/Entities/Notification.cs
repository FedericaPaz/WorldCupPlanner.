namespace Domain.Entities;

public class Notification
{
    public string UserEmail { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }
    public bool IsRead { get; private set; }

    public Notification(string userEmail, string message, DateTime createdAt)
    {
        UserEmail = userEmail;
        Message = message;
        CreatedAt = createdAt;
        IsRead = false;
    }
    
    public void MarkAsRead()
    {
        IsRead = true;
    }
}