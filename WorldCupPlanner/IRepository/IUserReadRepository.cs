using Domain.Entities;

namespace IRepository;

public interface IUserReadRepository
{
    User? GetUser(string email);
    List<User> GetAllUsers();
}