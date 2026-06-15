using Domain.Entities;

namespace IRepository;

public interface IUserRepository : IUserReadRepository
{
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
}