using Domain;
using DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class AuthServiceTests
{
    [TestMethod]
    public void Login_WithValidCredentials_ShouldReturnLoginResponse()
    {
        var userRepository = new UserRepository();
        var authService = new AuthService(userRepository);

        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", new List<UserRole> { UserRole.SystemAdministrator }
        );

        userRepository.AddUser(user);

        var request = new LoginRequest
        {
            Email = "juan.perez@gmail.com",
            Password = "Password1!"
        };

        var response = authService.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual("Juan Perez", response.Name);
        Assert.AreEqual("juan.perez@gmail.com", response.Email);
    }
}