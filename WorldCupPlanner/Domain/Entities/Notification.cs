namespace Domain.Entities;
using Domain.Interfaces;

public class Notification
{
    private DateTime _timestamp;
    private string _message;
    private string _recipientEmail;
    private bool _isRead;

    public DateTime Timestamp
    {
        get => _timestamp;
    }
    
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
    
    public Notification(string recipientEmail, string message, IDateTimeMatch? clock = null)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException("El destinatario es requerido.");
        
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("El mensaje es requerido.");
        
        _timestamp = clock?.Now ?? DateTime.Now;
        _message = message;
        _recipientEmail = recipientEmail;
        _isRead = false;
    }

    public void MarkAsRead()
    {
        _isRead = true;
    }
}