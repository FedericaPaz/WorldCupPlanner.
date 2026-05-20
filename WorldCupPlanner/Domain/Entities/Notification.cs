namespace Domain.Entities;

public class Notification
{
    private string _message;
    private string _recipientEmail;
    private bool _isRead;

    public string Message
    {
        get => _message;
    }
    
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
        _message = message;
        _recipientEmail = recipientEmail;
        _isRead = false;
    }
}