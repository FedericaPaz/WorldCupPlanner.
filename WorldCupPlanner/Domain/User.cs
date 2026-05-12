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

    private readonly IDateTimeMatch? _clock;

    public User(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        string password,
        List<UserRole>? roles = null,
        IDateTimeMatch? clock = null)
    {
        _clock = clock;

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

    public void UpdateData(
        string firstName,
        string lastName,
        DateTime birthDate,
        List<UserRole> roles)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateBirthDate(birthDate);
        ValidateRoles(roles);

        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Roles = roles;
    }

    public bool HasPassword(string password)
    {
        return PasswordHash == HashPassword(password);
    }

    public void ChangePassword(string newPassword)
    {
        ValidatePassword(newPassword);
        PasswordHash = HashPassword(newPassword);
    }

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío.");
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío.");
    }

    private static void ValidateEmailNotEmpty(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío.");
    }

    private void ValidateBirthDate(DateTime birthDate)
    {
        DateTime today = _clock?.Now.Date ?? DateTime.Today;

        if (birthDate > today)
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.");
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía.");

        if (password.Length < 8)
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("La contraseña debe contener al menos una letra mayúscula.");

        if (!password.Any(char.IsLower))
            throw new ArgumentException("La contraseña debe contener al menos una letra minúscula.");

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("La contraseña debe contener al menos un número.");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            throw new ArgumentException("La contraseña debe contener al menos un carácter especial.");
    }

    private static void ValidateEmailFormat(string email)
    {
        if (!email.Contains("@"))
            throw new ArgumentException("El formato del email no es válido.");
    }

    private static void ValidateRoles(List<UserRole>? roles)
    {
        if (roles is null || roles.Count == 0)
            throw new ArgumentException("El usuario debe tener al menos un rol asignado.");

        if (roles.Count != roles.Distinct().Count())
            throw new ArgumentException("El usuario no puede tener roles duplicados.");
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}