using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services;
using DTOs.User;

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
    
    [TestMethod]
    public void DeleteUser_WithNonExistingEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        Assert.ThrowsException<InvalidOperationException>(() =>
            userService.DeleteUser("juan.perez@gmail.com"));
    }
    
    [TestMethod]
    public void UpdateUser_WithExistingEmail_ShouldUpdateUser()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        userService.UpdateUser(
            "Juan",
            "Gomez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        var result = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Gomez", result.LastName);
    }
    
    [TestMethod]
    public void UpdateUser_WithNonExistingEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        Assert.ThrowsException<InvalidOperationException>(() =>
            userService.UpdateUser(
                "Juan",
                "Perez",
                "juan.perez@gmail.com",
                new DateTime(2000, 1, 1),
                "Password1!"));
    }
    
    [TestMethod]
    public void UpdateUser_WithEmptyEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        Assert.ThrowsException<ArgumentException>(() =>
            userService.UpdateUser(
                "Juan",
                "Perez",
                "",
                new DateTime(2000, 1, 1),
                "Password1!"));
    }
    
    [TestMethod]
    public void GetUser_WithEmptyEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        Assert.ThrowsException<ArgumentException>(() => userService.GetUser(""));
    }

    [TestMethod]
    public void DeleteUser_WithEmptyEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        Assert.ThrowsException<ArgumentException>(() => userService.DeleteUser(""));
    }
    
    [TestMethod]
    public void GetUser_WithExistingEmail_ShouldReturnUser()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!");

        var result = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
    }
    
    [TestMethod]
    public void CreateUser_WithValidDto_ShouldCreateUser()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        var userDto = new UserRequestDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!"
        };

        userService.CreateUser(userDto);

        var createdUser = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(createdUser);
        Assert.AreEqual("Juan", createdUser.FirstName);
        Assert.AreEqual("Perez", createdUser.LastName);
        Assert.AreEqual("juan.perez@gmail.com", createdUser.Email);
    }
    
    [TestMethod]
    public void GetUserDto_WithExistingUser_ShouldReturnUserResponseDto()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!");

        var result = userService.GetUserDto("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
        Assert.AreEqual(new DateTime(2000, 1, 1), result.BirthDate);
    }
}