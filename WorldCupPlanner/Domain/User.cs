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
    public List<UserRole> Roles { get; private set; }

    public User(string firstName, string lastName, string email, DateTime birthDate, string password, List<UserRole>? roles = null)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateEmailNotEmpty(email);
        ValidateBirthDate(birthDate);
        ValidatePassword(password);
        ValidateEmailFormat(email);
        ValidateRoles(roles);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
        PasswordHash = HashPassword(password);
        Roles = roles ?? new List<UserRole>();
    }

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("El nombre no puede estar vacío.");
        }
    }

    private void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("El apellido no puede estar vacío.");
        }
    }

    private void ValidateEmailNotEmpty(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email no puede estar vacío.");
        }
    }

    private void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate > DateTime.Today)
        {
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.");
        }
    }

    private void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La contraseña no puede estar vacía.");
        }

        if (password.Length < 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("La contraseña debe contener al menos una letra mayúscula.");
        }

        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("La contraseña debe contener al menos una letra minúscula.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("La contraseña debe contener al menos un número.");
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new ArgumentException("La contraseña debe contener al menos un carácter especial.");
        }
    }

    private void ValidateEmailFormat(string email)
    {
        if (!email.Contains("@"))
        {
            throw new ArgumentException("El formato del email no es válido.");
        }
    }
    
    private void ValidateRoles(List<UserRole>? roles)
    {
        if (roles is not null && roles.Count != roles.Distinct().Count())
        {
            throw new ArgumentException("El usuario no puede tener roles duplicados.");
        }
    }
    private static string HashPassword(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

