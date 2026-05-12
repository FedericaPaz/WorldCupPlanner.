using System.Security.Cryptography;
using System.Text;
using DTOs;
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

        var user = _userRepository
            .GetAllUsers()
            .FirstOrDefault(p => p.Email == request.Email && p.PasswordHash == HashPassword(request.Password));

        Console.WriteLine(_userRepository.GetAllUsers().Count);
        Console.WriteLine($"{request.Password} ----- {user?.PasswordHash}");

        if (user is null)
            throw new InvalidOperationException("Credenciales Inválidas.");

        return new LoginResponse 
        { 
            Name = $"{user.FirstName} {user.LastName}", 
            Email = user.Email 
        };
    }

    public void Register(RegisterRequest request)
    {
        var exists = _userRepository.GetAllUsers().Any(p => p.Email == request.Email);
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