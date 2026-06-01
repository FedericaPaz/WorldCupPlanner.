using Domain.Enums;

namespace DomainTests;

[TestClass]
public class UserRoleTests
{
    [TestMethod]
    public void UserRole_CheckingIfJournalistExists_ShouldReturnTrue()
    {
        bool exists = Enum.IsDefined(typeof(UserRole), UserRole.Journalist);
        Assert.IsTrue(exists);
    }
}