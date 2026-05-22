using System.Security.Cryptography;
using System.Text;
using DTOs.Auth;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public LoginResponse Login(LoginRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var passwordHash = HashPassword(request.Password);
        var user = _userRepository
            .GetAllUsers()
            .FirstOrDefault(user => user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && user.PasswordHash == passwordHash);

        if (user is null)
            throw new InvalidOperationException("Credenciales Inválidas.");

        return new LoginResponse
        {
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Roles = UserRoleMapper.ToDto(user.Roles.ToList())
        };
    }

    public void Register(RegisterRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.Password))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(request.Password));

        var exists = _userRepository.GetAllUsers().Any(user => user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (exists)
            throw new InvalidOperationException("Ya existe un usuario con ese email.");

        var user = UserMapper.ToEntity(request);
        _userRepository.AddUser(user);
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}