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
            throw new ArgumentException($"Ya existe un usuario con el email {user.Email}.");
        }

        _users.Add(user);
    }

    public User? GetUser(string email)
    {
        return _users.FirstOrDefault(user => user.Email == email);
    }

    public void UpdateUser(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var existingUser = _users.FirstOrDefault(u => u.Email == user.Email);

        if (existingUser is null)
        {
            throw new InvalidOperationException($"No se encontró un usuario con el email {user.Email}.");
        }

        _users.Remove(existingUser);
        _users.Add(user);
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
            throw new InvalidOperationException($"No se encontró un usuario con el email {user.Email}.");
        }

        _users.Remove(existingUser);
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }
}