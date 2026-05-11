using Domain;

namespace DomainTests.TestHelpers;

public static class UserTestHelper
{
    public static List<UserRole> GetValidRoles()
    {
        return new List<UserRole>
        {
            UserRole.SystemAdministrator
        };
    }
}