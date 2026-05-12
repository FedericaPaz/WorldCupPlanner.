using DTOs;
using Domain;

namespace Services.Mappers;

public static class UserMapper
{
    public static User ToEntity(RegisterRequest request)
    {
        var roles = UserRoleMapper.ToDomain(request.Roles);

        return new User(
            request.FirstName,
            request.LastName,
            request.Email,
            request.BirthDate,
            request.Password,
            roles
        );
    }
}