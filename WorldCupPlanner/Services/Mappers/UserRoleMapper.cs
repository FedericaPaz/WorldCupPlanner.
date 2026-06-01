using DTOs.User;
using Domain.Enums;

namespace Services.Mappers;

public static class UserRoleMapper
{
    public static UserRole ToDomain(UserRoleDto role)
    {
        return role switch
        {
            UserRoleDto.SystemAdministrator => UserRole.SystemAdministrator,
            UserRoleDto.Editor => UserRole.Editor,
            UserRoleDto.Journalist => UserRole.Journalist,
            _ => throw new ArgumentException("Rol inválido.")
        };
    }

    public static UserRoleDto ToDto(UserRole role)
    {
        return role switch
        {
            UserRole.SystemAdministrator => UserRoleDto.SystemAdministrator,
            UserRole.Editor => UserRoleDto.Editor,
            UserRole.Journalist => UserRoleDto.Journalist,
            _ => throw new ArgumentException("Rol inválido.")
        };
    }

    public static List<UserRole> ToDomain(List<UserRoleDto> roles)
    {
        return roles.Select(ToDomain).ToList();
    }

    public static List<UserRoleDto> ToDto(List<UserRole> roles)
    {
        return roles.Select(ToDto).ToList();
    }

    public static List<UserRoleDto> ToDto(IReadOnlyList<UserRole> roles)
    {
        return roles.Select(ToDto).ToList();
    }
}