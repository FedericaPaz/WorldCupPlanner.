using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;
using DTOs.User;
using Repository;
using Services;
using ServicesTests.TestHelpers;

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
            "Password1!",
            UserTestHelper.GetValidRoles()
        );

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
            "Password1!",
            UserTestHelper.GetValidRoles());

        Assert.ThrowsException<ArgumentException>(() =>
            userService.CreateUser(
                "Pedro",
                "Gomez",
                "juan.perez@gmail.com",
                new DateTime(1999, 5, 10),
                "Password1!",
                UserTestHelper.GetValidRoles()));
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
            "Password1!",
            UserTestHelper.GetValidRoles());

        userService.CreateUser(
            "Pedro",
            "Gomez",
            "pedro.gomez@gmail.com",
            new DateTime(1999, 5, 10),
            "Password1!",
            UserTestHelper.GetValidRoles());

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
            "Password1!",
            UserTestHelper.GetValidRoles());

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
            "Password1!",
            UserTestHelper.GetValidRoles());

        userService.UpdateUser(
            "Juan",
            "Gomez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!",
            UserTestHelper.GetValidRoles());

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
                "Password1!",
                UserTestHelper.GetValidRoles()));
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
                "Password1!",
                UserTestHelper.GetValidRoles()));
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
            "Password1!",
            UserTestHelper.GetValidRoles());

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
            Password = "Password1!",
            Roles = UserTestHelper.GetValidRolesDto()
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

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!",
            UserTestHelper.GetValidRoles());

        var result = userService.GetUserDto("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
        Assert.AreEqual(new DateTime(2000, 1, 1), result.BirthDate);
    }
    
    [TestMethod]
    public void CreateUser_WithRolesDto_ShouldCreateUserWithRoles()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        var userDto = new UserRequestDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!",
            Roles = new List<UserRoleDto>
            {
                UserRoleDto.SystemAdministrator,
                UserRoleDto.Editor
            }
        };

        userService.CreateUser(userDto);

        var createdUser = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(createdUser);
        Assert.AreEqual(2, createdUser.Roles.Count);
        CollectionAssert.Contains(createdUser.Roles, UserRole.SystemAdministrator);
        CollectionAssert.Contains(createdUser.Roles, UserRole.Editor);
    }
    
    [TestMethod]
    public void UpdateUser_WithRoles_ShouldUpdateUserRoles()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        userService.CreateUser(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!",
            UserTestHelper.GetValidRoles()
        );

        var roles = new List<UserRole>
        {
            UserRole.SystemAdministrator,
            UserRole.Editor
        };

        userService.UpdateUser(
            "Juan",
            "Gomez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!",
            roles
        );

        var updatedUser = userService.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(updatedUser);
        Assert.AreEqual(2, updatedUser.Roles.Count);
        CollectionAssert.Contains(updatedUser.Roles, UserRole.SystemAdministrator);
        CollectionAssert.Contains(updatedUser.Roles, UserRole.Editor);
    }
    
    [TestMethod]
    public void GetUserDto_WithRoles_ShouldReturnRoles()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        var roles = new List<UserRoleDto>
        {
            UserRoleDto.SystemAdministrator,
            UserRoleDto.Editor
        };

        var userDto = new UserRequestDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!",
            Roles = roles
        };

        userService.CreateUser(userDto);

        var result = userService.GetUserDto("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Roles.Count);
        CollectionAssert.Contains(result.Roles, UserRoleDto.SystemAdministrator);
        CollectionAssert.Contains(result.Roles, UserRoleDto.Editor);
    }
    
    [TestMethod]
    public void ResetPassword_WithExistingUser_ShouldUpdatePassword()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);
        userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        userService.ResetPassword("juan.perez@gmail.com", "NewPassword1!");
        var user = userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(user);
        Assert.IsTrue(user.HasPassword("NewPassword1!"));
    }
    
    [TestMethod]
    public void ChangeOwnPassword_WithExistingUser_ShouldUpdatePassword()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);
        userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        userService.ChangeOwnPassword("juan.perez@gmail.com", "NewPassword1!");
        var user = userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(user);
        Assert.IsTrue(user.HasPassword("NewPassword1!"));
        
    }
}