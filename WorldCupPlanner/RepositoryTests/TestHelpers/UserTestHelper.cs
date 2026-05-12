using Domain;

namespace RepositoryTests.TestHelpers;

public class UserTestHelper
{
    public static List<UserRole> GetValidRoles()
    {
        return new List<UserRole>
        {
            UserRole.SystemAdministrator
        };
    }

    public static User CreateValidUser(
        string firstName,
        string lastName,
        string email,
        DateTime? birthDate = null)
    {
        return new User(
            firstName,
            lastName,
            email,
            birthDate ?? new DateTime(2000, 1, 1),
            "Password1!",
            GetValidRoles()
        );
    }
}