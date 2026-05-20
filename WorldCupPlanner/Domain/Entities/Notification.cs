namespace Domain.Entities;

public class Notification
{
    private string _recipientEmail;
    private bool _isRead;

    public string RecipientEmail
    {
        get => _recipientEmail;
    }
    
    public bool IsRead
    {
        get => _isRead;
    }
    
    public Notification(string recipientEmail, string message)
    {
        _recipientEmail = recipientEmail;
        _isRead = false;
    }
}