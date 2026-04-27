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

    [TestMethod]
    public void CreateUser_WithExistingEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        Assert.ThrowsException<ArgumentException>(() =>
            userService.CreateUser(
                "Pedro",
                "Gomez",
                "juan.perez@gmail.com",
                new DateTime(1999, 5, 10),
                "Password1!"));
    }

    [TestMethod]
    public void GetAllUsers_ShouldReturnCreatedUsers()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        userService.CreateUser(
            "Pedro",
            "Gomez",
            "pedro.gomez@gmail.com",
            new DateTime(1999, 5, 10),
            "Password1!");

        var result = userService.GetAllUsers();

        Assert.AreEqual(2, result.Count);
    }
    
    [TestMethod]
    public void GetUser_WithNonExistingEmail_ShouldReturnNull()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        var result = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void DeleteUser_WithExistingEmail_ShouldDeleteUser()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        userService.DeleteUser("juan.perez@gmail.com");

        var result = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNull(result);
    }
}