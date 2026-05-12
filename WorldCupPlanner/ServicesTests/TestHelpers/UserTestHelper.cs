using Domain;
using DTOs.User;

namespace ServicesTests.TestHelpers;

public static class UserTestHelper
{
    public static List<UserRole> GetValidRoles()
    {
        return new List<UserRole>
        {
            UserRole.SystemAdministrator
        };
    }

    public static List<UserRoleDto> GetValidRolesDto()
    {
        return new List<UserRoleDto>
        {
            UserRoleDto.SystemAdministrator
        };
    }

    public static UserRequestDto CreateValidUserRequestDto()
    {
        return new UserRequestDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!",
            Roles = GetValidRolesDto()
        };
    }
}