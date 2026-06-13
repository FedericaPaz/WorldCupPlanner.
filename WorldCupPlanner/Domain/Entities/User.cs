using System.Security.Cryptography;
using System.Text;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class User
{
    private const int MinPasswordLength = 8;

    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private DateTime _birthDate;
    private string _passwordHash = string.Empty;
    private List<UserRole> _roles = new();
    private readonly IDateTimeMatch? _clock;

    public string FirstName
    {
        get { return _firstName; }
        private set
        {
            EnsureRequiredText(value, "El nombre no puede estar vacío.", nameof(FirstName));
            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get { return _lastName; }
        private set
        {
            EnsureRequiredText(value, "El apellido no puede estar vacío.", nameof(LastName));
            _lastName = value.Trim();
        }
    }

    public string Email
    {
        get { return _email; }
        private set
        {
            EnsureValidEmail(value);
            _email = value.Trim();
        }
    }

    public DateTime BirthDate
    {
        get { return _birthDate; }
        private set
        {
            EnsureValidBirthDate(value);
            _birthDate = value;
        }
    }

    public string PasswordHash
    {
        get { return _passwordHash; }
        private set { _passwordHash = value; }
    }

    public IReadOnlyList<UserRole> Roles
    {
        get { return _roles; }
    }

    private User()
    {
        
    }
    
    public User(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        string password,
        List<UserRole> roles,
        IDateTimeMatch? clock = null)
    {
        _clock = clock;

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
        ChangePassword(password);
        SetRoles(roles);
    }

    public void UpdateData(
        string firstName,
        string lastName,
        DateTime birthDate,
        List<UserRole> roles)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        SetRoles(roles);
    }

    public bool HasPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    public void ChangePassword(string newPassword)
    {
        EnsureValidPassword(newPassword);
        PasswordHash = HashPassword(newPassword);
    }

    private void SetRoles(List<UserRole> roles)
    {
        EnsureValidRoles(roles);
        _roles = new List<UserRole>(roles);
    }

    private static void EnsureRequiredText(string value, string message, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, parameterName);
        }
    }

    private static void EnsureValidEmail(string email)
    {
        EnsureRequiredText(email, "El email no puede estar vacío.", nameof(email));

        if (!email.Contains("@"))
        {
            throw new ArgumentException("El formato del email no es válido.", nameof(email));
        }
    }

    private void EnsureValidBirthDate(DateTime birthDate)
    {
        DateTime today = _clock?.Now.Date ?? DateTime.Today;

        if (birthDate > today)
        {
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(birthDate));
        }
    }

    private static void EnsureValidPassword(string password)
    {
        EnsureRequiredText(password, "La contraseña no puede estar vacía.", nameof(password));
        EnsurePasswordHasValidLength(password);
        EnsurePasswordHasUppercase(password);
        EnsurePasswordHasLowercase(password);
        EnsurePasswordHasNumber(password);
        EnsurePasswordHasSpecialCharacter(password);
    }

    private static void EnsurePasswordHasValidLength(string password)
    {
        if (password.Length < MinPasswordLength)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.", nameof(password));
        }
    }

    private static void EnsurePasswordHasUppercase(string password)
    {
        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("La contraseña debe contener al menos una letra mayúscula.", nameof(password));
        }
    }

    private static void EnsurePasswordHasLowercase(string password)
    {
        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("La contraseña debe contener al menos una letra minúscula.", nameof(password));
        }
    }

    private static void EnsurePasswordHasNumber(string password)
    {
        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("La contraseña debe contener al menos un número.", nameof(password));
        }
    }

    private static void EnsurePasswordHasSpecialCharacter(string password)
    {
        if (!password.Any(character => !char.IsLetterOrDigit(character)))
        {
            throw new ArgumentException("La contraseña debe contener al menos un carácter especial.", nameof(password));
        }
    }

    private static void EnsureValidRoles(List<UserRole> roles)
    {
        if (roles == null || roles.Count == 0)
        {
            throw new ArgumentException("El usuario debe tener al menos un rol asignado.", nameof(roles));
        }

        if (roles.Count != roles.Distinct().Count())
        {
            throw new ArgumentException("El usuario no puede tener roles duplicados.", nameof(roles));
        }
    }

    private static string HashPassword(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashedPassword = SHA256.HashData(passwordBytes);

        return Convert.ToBase64String(hashedPassword);
    }
    
    public void SyncPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
    }
    
    public bool CanExport()
    {
        return _roles.Contains(UserRole.SystemAdministrator) ||
               _roles.Contains(UserRole.Journalist);
    }
    
    public static string GenerateDefaultPassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specials = "!@#$%^&*()-_+=";

        var rnd = new Random();
        var all = upper + lower + digits + specials;
        var pwd = new char[10];

        pwd[0] = upper[rnd.Next(upper.Length)];
        pwd[1] = lower[rnd.Next(lower.Length)];
        pwd[2] = digits[rnd.Next(digits.Length)];
        pwd[3] = specials[rnd.Next(specials.Length)];

        for (int i = 4; i < pwd.Length; i++)
            pwd[i] = all[rnd.Next(all.Length)];

        return new string(pwd.OrderBy(x => rnd.Next()).ToArray());
    }
}