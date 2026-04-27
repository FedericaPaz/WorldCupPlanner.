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
        return _userRepository.GetUser(email);
    }
    
    public void DeleteUser(string email)
    {
        var user = _userRepository.GetUser(email);
        _userRepository.DeleteUser(user);
    }
}