using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly WorldCupPlannerContext _context;

    public UserRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddUser(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (_context.Users.Any(u => u.Email == user.Email))
            throw new ArgumentException($"Ya existe un usuario con el email {user.Email}.");

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? GetUser(string email)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email == email);
    }

    public void UpdateUser(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        var existing = _context.Users.FirstOrDefault(u => u.Email == user.Email)
                       ?? throw new InvalidOperationException($"No se encontró un usuario con el email {user.Email}.");

        existing.UpdateData(
            user.FirstName,
            user.LastName,
            user.BirthDate,
            user.Roles.ToList());
        
        existing.SyncPasswordHash(user.PasswordHash);

        _context.SaveChanges();
    }

    public void DeleteUser(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        var existing = _context.Users.FirstOrDefault(u => u.Email == user.Email)
                       ?? throw new InvalidOperationException($"No se encontró un usuario con el email {user.Email}.");

        _context.Users.Remove(existing);
        _context.SaveChanges();
    }

    public List<User> GetAllUsers()
    {
        return _context.Users
            .AsNoTracking()
            .ToList();
    }
}