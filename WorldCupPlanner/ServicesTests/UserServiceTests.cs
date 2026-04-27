using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class UserServiceTests
{
    [TestMethod]
    public void CreateUser_WithValidData_ShouldAddUser()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        var users = userService.GetAllUsers();

        Assert.AreEqual(1, users.Count);
    }
}