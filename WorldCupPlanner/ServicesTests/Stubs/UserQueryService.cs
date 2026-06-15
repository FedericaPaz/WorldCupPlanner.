using Domain.Enums;
using DTOs.User;
using IServices.UserInterfaces;
using Services.Mappers;

namespace ServicesTests.Stubs;

public class UserQueryServiceStub : IUserQueryService
{
    private readonly List<UserResponseDto> _users = new();

    public void AddUser(string email, UserRole role)
    {
        _users.Add(new UserResponseDto
        {
            Email = email,
            Roles = new List<UserRoleDto> { UserRoleMapper.ToDto(role) }
        });
    }

    public List<UserResponseDto> GetAllUsers() => _users.ToList();

    public UserResponseDto? GetUserDto(string email) =>
        _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public bool CanExport(string email)
    {
        var user = GetUserDto(email);
        if (user is null) throw new InvalidOperationException("El usuario no existe.");
        return user.Roles.Contains(UserRoleDto.SystemAdministrator) ||
               user.Roles.Contains(UserRoleDto.Journalist);
    }
}