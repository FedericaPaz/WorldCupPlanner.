using Domain;
using IRepository;
using DTOs.User;

namespace Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void CreateUser(string firstName, string lastName, string email, DateTime birthDate, string password)
    {
        var user = new User(firstName, lastName, email, birthDate, password);
        _userRepository.AddUser(user);
    }

    public void CreateUser(UserRequestDto userDto)
    {
        var user = new User(
            userDto.FirstName,
            userDto.LastName,
            userDto.Email,
            userDto.BirthDate,
            userDto.Password,
            userDto.Roles
        );

        _userRepository.AddUser(user);
    }
    
    public List<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
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
            Roles = user.Roles
        };
    }
    
    public void DeleteUser(string email)
    {
        ValidateEmailNotEmpty(email);
        var user = GetExistingUserOrThrow(email);
        _userRepository.DeleteUser(user);
    }
    
    public void UpdateUser(
        string firstName,
        string lastName,
        string email,
        DateTime birthDate,
        string password,
        List<UserRole>? roles = null)
    {
        ValidateEmailNotEmpty(email);
        var existingUser = GetExistingUserOrThrow(email);

        var user = new User(firstName, lastName, email, birthDate, password, roles);
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