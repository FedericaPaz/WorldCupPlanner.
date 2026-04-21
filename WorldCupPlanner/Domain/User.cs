using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Domain;

public class User
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string PasswordHash { get; private set; }

    public User(string firstName, string lastName, string email, DateTime birthDate, string password)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }
        
        if (birthDate > DateTime.Today)
        {
            throw new ArgumentException("Birth date cannot be in the future.");
        }
        
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.");
        }
        
        if (password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.");
        }
        
        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }
        
        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }
        
        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one number.");
        }
        
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new ArgumentException("Password must contain at least one special character.");
        }
        
        if (!email.Contains("@"))
        {
            throw new ArgumentException("Email format is invalid.");
        }
        
        
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
        PasswordHash = HashPassword(password);
    }
    private static string HashPassword(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

