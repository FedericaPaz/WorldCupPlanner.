using Domain;
using DTOs.User;

namespace Services.Mappers;

public static class UserRoleMapper
{
    public static UserRole ToDomain(UserRoleDto role)
    {
        return role switch
        {
            UserRoleDto.Admin => UserRole.SystemAdministrator,
            UserRoleDto.Editor => UserRole.Editor,
            _ => throw new ArgumentException("Rol inválido.")
        };
    }

    public static UserRoleDto ToDto(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => UserRoleDto.Admin,
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