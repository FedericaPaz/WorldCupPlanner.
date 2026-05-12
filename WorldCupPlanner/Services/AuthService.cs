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
        _userRepository = userRepository;
    }

    public LoginResponse Login(LoginRequest request)
    {
        var users = _userRepository.GetAllUsers();
        var user = users.FirstOrDefault(p => p.Email == request.Email && p.PasswordHash == HashPassword(request.Password));

        if (user is null)
            throw new InvalidOperationException("Credenciales Inválidas.");

        return new LoginResponse 
        { 
            Name = $"{user.FirstName} {user.LastName}", 
            Email = user.Email,
            Roles = UserRoleMapper.ToDto(user.Roles)
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