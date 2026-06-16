using Domain.Entities;
using Domain.Enums;
using IRepository;

namespace ServicesTests.Stubs;

public class UserRepositoryStub : IUserRepository
{
    private readonly List<User> _users = new();

    public void AddUser(string email, UserRole role)
    {
        var user = new User("Test", "User", email, DateTime.Now.AddYears(-20), "Password1!", new List<UserRole> { role });
        _users.Add(user);
    }

    public void AddUser(User user) => _users.Add(user);

    public User? GetUser(string email) =>
        _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public void UpdateUser(User user) { }

    public void DeleteUser(User user) => _users.Remove(user);

    public List<User> GetAllUsers() => _users.ToList();
}
