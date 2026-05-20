namespace Domain.Entities;

public class Notification
{
    private bool _isRead;

    public bool IsRead
    {
        get => _isRead;
    }
    
    public Notification(string recipientEmail, string message)
    {
        _isRead = false;
    }
}