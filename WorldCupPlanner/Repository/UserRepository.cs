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

        _users.Add(user);
    }

    public User? GetUser(string email)
    {
        throw new NotImplementedException();
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(User user)
    {
        throw new NotImplementedException();
    }

    public List<User> GetAllUsers()
    {
        throw new NotImplementedException();
    }
}