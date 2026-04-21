namespace Domain;
using System.Linq;
public class User
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string Password { get; private set; }

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
        
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
        Password = password;
    }
}