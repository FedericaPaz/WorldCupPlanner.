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

    public void CreateUser(UserCreateDto userDto)
    {
        CreateUser(
            userDto.FirstName,
            userDto.LastName,
            userDto.Email,
            userDto.BirthDate,
            userDto.Password
        );
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
            BirthDate = user.BirthDate
        };
    }
    
    public void DeleteUser(string email)
    {
        ValidateEmailNotEmpty(email);
        var user = GetExistingUserOrThrow(email);
        _userRepository.DeleteUser(user);
    }
    
    public void UpdateUser(string firstName, string lastName, string email, DateTime birthDate, string password)
    {
        ValidateEmailNotEmpty(email);
        var existingUser = GetExistingUserOrThrow(email);

        var user = new User(firstName, lastName, email, birthDate, password);
        _userRepository.UpdateUser(user);
    }

    private void ValidateEmailNotEmpty(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }
    }

    private User GetExistingUserOrThrow(string email)
    {
        var user = _userRepository.GetUser(email);
        if (user is null)
        {
            throw new InvalidOperationException($"User with email {email} not found");
        }
        return user;
    }
    
}