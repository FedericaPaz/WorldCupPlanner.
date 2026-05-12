using Domain;
using DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Services;

namespace ServicesTests;

[TestClass]
public class AuthServiceTests
{
    private static (UserRepository Repo, AuthService Service, User User) Setup(string firstName = "Juan", string lastName = "Perez", string email = "juan.perez@gmail.com", string password = "Password1!")
    {
        var repo = new UserRepository();
        var service = new AuthService(repo);
        var user = new User(firstName, lastName, email, new DateTime(2000, 1, 1), password, new List<UserRole> { UserRole.SystemAdministrator });
        repo.AddUser(user);
        return (repo, service, user);
    }

    [TestMethod]
    public void Login_WithValidCredentials_ShouldReturnLoginResponse()
    {
        var (_, authService, user) = Setup();

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "Password1!"
        };

        var response = authService.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual("Juan Perez", response.Name);
        Assert.AreEqual("juan.perez@gmail.com", response.Email);
    }

    [TestMethod]
    public void Login_WithInvalidPassword_ShouldThrowException()
    {
        var (_, authService, user) = Setup();

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "WrongPassword1!"
        };

        Assert.ThrowsException<InvalidOperationException>(() => authService.Login(request));
    }

    [TestMethod]
    public void Login_WithNonExistingEmail_ShouldThrowException()
    {
        var (_, authService, _) = Setup();

        var request = new LoginRequest
        {
            Email = "noexiste@gmail.com",
            Password = "Password1!"
        };

        Assert.ThrowsException<InvalidOperationException>(() => authService.Login(request));
    }

    [TestMethod]
    public void Login_WithValidCredentials_ShouldReturnFullName()
    {
        var (_, authService, _) = Setup("Pedro", "Gomez", "pedro.gomez@gmail.com", "Password1!");

        var request = new LoginRequest
        {
            Email = "pedro.gomez@gmail.com",
            Password = "Password1!"
        };

        var response = authService.Login(request);

        Assert.AreEqual("Pedro Gomez", response.Name);
    }

    [TestMethod]
    public void Login_WithEmptyEmail_ShouldThrowException()
    {
        var (_, authService, _) = Setup();

        var request = new LoginRequest
        {
            Email = "",
            Password = "Password1!"
        };

        Assert.ThrowsException<InvalidOperationException>(() => authService.Login(request));
    }

    [TestMethod]
    public void Login_WithEmptyPassword_ShouldThrowException()
    {
        var (_, authService, _) = Setup();

        var request = new LoginRequest
        {
            Email = "juan.perez@gmail.com",
            Password = ""
        };

        Assert.ThrowsException<InvalidOperationException>(() => authService.Login(request));
    }

    [TestMethod]
    public void Login_WithNullRequest_ShouldThrowArgumentNullException()
    {
        var (_, authService, _) = Setup();

        Assert.ThrowsException<ArgumentNullException>(() => authService.Login(null!));
    }

    [TestMethod]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new AuthService(null!));
    }

    [TestMethod]
    public void Login_WithDifferentEmailCasing_ShouldReturnLoginResponse()
    {
        var (_, authService, _) = Setup();

        var request = new LoginRequest
        {
            Email = "JUAN.PEREZ@gmail.com",
            Password = "Password1!"
        };

        var response = authService.Login(request);

        Assert.IsNotNull(response);
        Assert.AreEqual("Juan Perez", response.Name);
        Assert.AreEqual("juan.perez@gmail.com", response.Email);
    }
    
    [TestMethod]
    public void Register_WithValidRequest_ShouldAddUser()
    {
        var userRepository = new UserRepository();
        var authService = new AuthService(userRepository);

        var request = new RegisterRequest
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!"
        };

        authService.Register(request);

        var users = userRepository.GetAllUsers();

        Assert.AreEqual(1, users.Count);
        Assert.AreEqual("Juan", users[0].FirstName);
        Assert.AreEqual("Perez", users[0].LastName);
        Assert.AreEqual("juan.perez@gmail.com", users[0].Email);
    }
    
    [TestMethod]
    public void Register_WithValidRequest_ShouldCreateUserWithEditorRole()
    {
        var userRepository = new UserRepository();
        var authService = new AuthService(userRepository);

        var request = new RegisterRequest
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!"
        };

        authService.Register(request);

        var user = userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(user);
        Assert.AreEqual(1, user.Roles.Count);
        CollectionAssert.Contains(user.Roles, UserRole.Editor);
    }
    
    [TestMethod]
    public void Register_WithExistingEmail_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var authService = new AuthService(userRepository);

        var request = new RegisterRequest
        {
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!"
        };

        authService.Register(request);

        Assert.ThrowsException<InvalidOperationException>(() =>
            authService.Register(request));
    }
}