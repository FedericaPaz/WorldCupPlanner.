namespace Domain.Entities;
using Domain.Interfaces;

public class Notification
{
    private Guid _id;
    private DateTime _timestamp;
    private string _message;
    private string _recipientEmail;
    private bool _isRead;
    private DateTime? _readAt;

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
    
    public Guid Id
    {
        get => _id;
    }
    
    public DateTime? ReadAt
    {
        get => _readAt;
    }
    
    public Notification(
        string recipientEmail,
        string message,
        IDateTimeMatch? clock = null)
    {
        EnsureRecipientEmailIsValid(recipientEmail);
        EnsureMessageIsValid(message);
        
        _id = Guid.NewGuid();
        _recipientEmail = recipientEmail;
        _message = message;
        _timestamp = clock?.Now ?? DateTime.UtcNow;
        _isRead = false;
        _readAt = null;
    }

    public void MarkAsRead(IDateTimeMatch? clock = null)
    {
        _isRead = true;
        _readAt = clock?.Now ?? DateTime.UtcNow;
    }

    private static void EnsureRecipientEmailIsValid(string recipientEmail)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            throw new ArgumentException("El destinatario es requerido.");
        }
    }

    private static void EnsureMessageIsValid(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("El mensaje es requerido.");
        }
    }
}