using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTOs;
using Repository;
using Services;
using ServicesTests.TestHelpers;

namespace ServicesTests;

[TestClass]
public class AuthServiceTests
{
    [TestMethod]
    public void Register_WithValidRequest_ShouldCreateUser()
    {
        var repo = new UserRepository();
        var auth = new AuthService(repo);

        var req = new RegisterRequest
        {
            FirstName = "María",
            LastName = "González",
            Email = "maria.gonzalez@gmail.com",
            BirthDate = new DateTime(1990,1,1),
            Password = "Password1!",
            Roles = UserTestHelper.GetValidRolesDto()
        };

        auth.Register(req);

        var users = repo.GetAllUsers();
        Assert.AreEqual(1, users.Count);
        Assert.AreEqual(req.Email, users[0].Email);
    }

    [TestMethod]
    public void Register_WithExistingEmail_ShouldThrowException()
    {
        var repo = new UserRepository();
        var auth = new AuthService(repo);

        var req = new RegisterRequest
        {
            FirstName = "María",
            LastName = "González",
            Email = "maria.gonzalez@gmail.com",
            BirthDate = new DateTime(1990,1,1),
            Password = "Password1!",
            Roles = UserTestHelper.GetValidRolesDto()
        };

        auth.Register(req);

        Assert.ThrowsException<InvalidOperationException>(() => auth.Register(req));
    }

    [TestMethod]
    public void Login_WithValidCredentials_ShouldReturnLoginResponse()
    {
        var repo = new UserRepository();
        var auth = new AuthService(repo);

        var req = new RegisterRequest
        {
            FirstName = "Ana",
            LastName = "Lopez",
            Email = "ana.lopez@gmail.com",
            BirthDate = new DateTime(1992,2,2),
            Password = "Password1!",
            Roles = UserTestHelper.GetValidRolesDto()
        };

        auth.Register(req);

        var loginReq = new LoginRequest { Email = req.Email, Password = req.Password };
        var resp = auth.Login(loginReq);

        Assert.IsNotNull(resp);
        Assert.AreEqual("Ana Lopez", resp.Name);
        Assert.AreEqual(req.Email, resp.Email);
        CollectionAssert.Contains(resp.Roles, DTOs.User.UserRoleDto.SystemAdministrator);
    }

    [TestMethod]
    public void Login_WithInvalidPassword_ShouldThrowException()
    {
        var repo = new UserRepository();
        var auth = new AuthService(repo);

        var req = new RegisterRequest
        {
            FirstName = "Ana",
            LastName = "Lopez",
            Email = "ana.lopez2@gmail.com",
            BirthDate = new DateTime(1992,2,2),
            Password = "Password1!",
            Roles = UserTestHelper.GetValidRolesDto()
        };

        auth.Register(req);

        var loginReq = new LoginRequest { Email = req.Email, Password = "WrongPass1!" };
        Assert.ThrowsException<InvalidOperationException>(() => auth.Login(loginReq));
    }

    [TestMethod]
    public void Login_WithNonExistingUser_ShouldThrowException()
    {
        var repo = new UserRepository();
        var auth = new AuthService(repo);

        var loginReq = new LoginRequest { Email = "noone@gmail.com", Password = "Password1!" };
        Assert.ThrowsException<InvalidOperationException>(() => auth.Login(loginReq));
    }
}