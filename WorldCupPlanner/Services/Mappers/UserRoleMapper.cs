using DTOs.User;
using Domain;

namespace Services.Mappers;

public static class UserRoleMapper
{
    public static UserRole ToDomain(UserRoleDto role)
    {
        return role switch
        {
            UserRoleDto.SystemAdministrator => UserRole.SystemAdministrator,
            UserRoleDto.Editor => UserRole.Editor,
            _ => throw new ArgumentException("Rol inválido.")
        };
    }

    public static UserRoleDto ToDto(UserRole role)
    {
        return role switch
        {
            UserRole.SystemAdministrator => UserRoleDto.SystemAdministrator,
            UserRole.Editor => UserRoleDto.Editor,
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
}