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
    
    public Notification(
        string recipientEmail,
        string message,
        IDateTimeMatch? clock = null)
    {
        EnsureRecipientEmailIsValid(recipientEmail);
        EnsureMessageIsValid(message);

        _recipientEmail = recipientEmail;
        _message = message;
        _timestamp = clock?.Now ?? DateTime.UtcNow;
        _isRead = false;
    }

    public void MarkAsRead()
    {
        _isRead = true;
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