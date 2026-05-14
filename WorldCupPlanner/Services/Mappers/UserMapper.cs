using Domain;
using DTOs;
using DTOs.User;

namespace Services.Mappers;

public static class UserMapper
{
    public static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Roles = UserRoleMapper.ToDto(user.Roles.ToList())
        };
    }

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