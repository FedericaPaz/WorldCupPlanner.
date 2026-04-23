using Domain;
using IRepository;

namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public void AddUser(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (_users.Any(u => u.Email == user.Email))
        {
            throw new ArgumentException($"User with email {user.Email} already exists");
        }
        _users.Add(user);
    }

    public User? GetUser(string email)
    {
        return _users.FirstOrDefault(user => user.Email == email);
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        var existingUser = _users.FirstOrDefault(u => u.Email == user.Email);
        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with email {user.Email} not found");
        }
        _users.Remove(existingUser);
    }

    public List<User> GetAllUsers()
    {
        throw new NotImplementedException();
    }
}