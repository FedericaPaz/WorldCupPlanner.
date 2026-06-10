using Domain.Entities;
using Domain.Enums;
using IRepository;
using DTOs.User;
using Services.Mappers;

namespace Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void CreateUser(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        string password,
        List<UserRole> roles)
    {
        var user = new User(firstName, lastName, email, birthDate, password, roles);
        _userRepository.AddUser(user);
    }

    public void CreateUser(UserRequestDto userDto)
    {
        var roles = UserRoleMapper.ToDomain(userDto.Roles);
        var user = new User(
            userDto.FirstName,
            userDto.LastName,
            userDto.Email,
            userDto.BirthDate,
            userDto.Password,
            roles
        );

        _userRepository.AddUser(user);
    }

    public List<UserResponseDto> GetAllUsers()
    {
        return _userRepository.GetAllUsers()
            .Select(UserMapper.ToDto)
            .ToList();
    }

    public User? GetUser(string email)
    {
        ValidateEmailNotEmpty(email);
        return _userRepository.GetUser(email);
    }

    public UserResponseDto? GetUserDto(string email)
    {
        var user = GetUser(email);

        if (user is null)
        {
            return null;
        }

        return new UserResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Roles = UserRoleMapper.ToDto(user.Roles.ToList())
        };
    }

    public void DeleteUser(string email)
    {
        ValidateEmailNotEmpty(email);
        var user = GetExistingUserOrThrow(email);
        _userRepository.DeleteUser(user);
    }

    public void DeleteUser(string email, string currentUserEmail)
    {
        ValidateEmailNotEmpty(email);
        ValidateEmailNotEmpty(currentUserEmail);

        if (email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("No podés eliminar tu propio usuario.");
        }

        var user = GetExistingUserOrThrow(email);
        _userRepository.DeleteUser(user);
    }

    public void UpdateUser(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        string password,
        List<UserRole> roles)
    {
        ValidateEmailNotEmpty(email);
        GetExistingUserOrThrow(email);

        var user = new User(firstName, lastName, email, birthDate, password, roles);
        _userRepository.UpdateUser(user);
    }

    public void ResetPassword(string email, string defaultPassword)
    {
        ValidateEmailNotEmpty(email);
        var user = GetExistingUserOrThrow(email);
        user.ChangePassword(defaultPassword);
        _userRepository.UpdateUser(user);
    }

    public void ChangeOwnPassword(string email, string newPassword)
    {
        ValidateEmailNotEmpty(email);
        var user = GetExistingUserOrThrow(email);
        user.ChangePassword(newPassword);
        _userRepository.UpdateUser(user);
    }

    public string GenerateDefaultPassword()
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
        {
            pwd[i] = all[rnd.Next(all.Length)];
        }

        return new string(pwd.OrderBy(x => rnd.Next()).ToArray());
    }

    public void UpdateUserWithoutPassword(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        List<UserRole> roles)
    {
        ValidateEmailNotEmpty(email);
        var user = GetExistingUserOrThrow(email);
        user.UpdateData(firstName, lastName, birthDate, roles);
        _userRepository.UpdateUser(user);
    }

    private void ValidateEmailNotEmpty(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email no puede estar vacío.");
        }
    }

    private User GetExistingUserOrThrow(string email)
    {
        var user = _userRepository.GetUser(email);

        if (user is null)
        {
            throw new InvalidOperationException($"No se encontró un usuario con el email {email}.");
        }

        return user;
    }
}