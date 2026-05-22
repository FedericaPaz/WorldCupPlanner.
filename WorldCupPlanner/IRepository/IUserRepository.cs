using Domain.Entities;

namespace IRepository;

public interface IUserRepository
{
    void AddUser(User user);
    User? GetUser(string email);
    void UpdateUser(User user);
    void DeleteUser(User user);
    List<User> GetAllUsers();
}