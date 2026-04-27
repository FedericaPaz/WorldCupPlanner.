using Domain;
using IRepository;

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

    public List<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

    public User? GetUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        return _userRepository.GetUser(email);
    }
    
    public void DeleteUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        var user = _userRepository.GetUser(email);

        if (user is null)
        {
            throw new InvalidOperationException($"User with email {email} not found");
        }

        _userRepository.DeleteUser(user);
    }
    
    public void UpdateUser(string firstName, string lastName, string email, DateTime birthDate, string password)
    {
        var existingUser = _userRepository.GetUser(email);

        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with email {email} not found");
        }

        var user = new User(firstName, lastName, email, birthDate, password);
        _userRepository.UpdateUser(user);
    }
}