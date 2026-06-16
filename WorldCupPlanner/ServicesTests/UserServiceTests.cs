using Domain.Entities;
using Domain.Enums;
using DTOs.User;
using Repository;
using Services;
using ServicesTests.TestHelpers;

namespace ServicesTests;

[TestClass]
public class UserServiceTests
{
    private WorldCupPlannerContext _context = null!;
    private UserRepository _userRepository = null!;
    private UserService _userService = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextFactory.CreateInMemoryContext();
        _userRepository = new UserRepository(_context);
        _userService = new UserService(_userRepository);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void CreateUser_WithValidData_ShouldAddUser()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        Assert.AreEqual(1, _userService.GetAllUsers().Count);
    }

    [TestMethod]
    public void CreateUser_WithExistingEmail_ShouldThrowException()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser("Pedro", "Gomez", "juan.perez@gmail.com",
                new DateTime(1999, 5, 10), "Password1!", UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void GetAllUsers_ShouldReturnCreatedUsers()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        _userService.CreateUser("Pedro", "Gomez", "pedro.gomez@gmail.com",
            new DateTime(1999, 5, 10), "Password1!", UserTestHelper.GetValidRoles());

        Assert.AreEqual(2, _userService.GetAllUsers().Count);
    }

    [TestMethod]
    public void GetUser_WithNonExistingEmail_ShouldReturnNull()
    {
        Assert.IsNull(_userService.GetUser("juan.perez@gmail.com"));
    }

    [TestMethod]
    public void DeleteUser_WithExistingEmail_ShouldDeleteUser()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        _userService.DeleteUser("juan.perez@gmail.com");

        Assert.IsNull(_userService.GetUser("juan.perez@gmail.com"));
    }

    [TestMethod]
    public void DeleteUser_WithNonExistingEmail_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.DeleteUser("juan.perez@gmail.com"));
    }

    [TestMethod]
    public void UpdateUser_WithExistingEmail_ShouldUpdateUser()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        _userService.UpdateUser("Juan", "Gomez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        var result = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual("Gomez", result.LastName);
    }

    [TestMethod]
    public void UpdateUser_WithNonExistingEmail_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.UpdateUser("Juan", "Perez", "juan.perez@gmail.com",
                new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void UpdateUser_WithEmptyEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser("Juan", "Perez", "",
                new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void GetUser_WithEmptyEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => _userService.GetUser(""));
    }

    [TestMethod]
    public void DeleteUser_WithEmptyEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => _userService.DeleteUser(""));
    }

    [TestMethod]
    public void GetUser_WithExistingEmail_ShouldReturnUser()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        var result = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
    }

    [TestMethod]
    public void CreateUser_WithValidDto_ShouldCreateUser()
    {
        var userDto = new UserRequestDto
        {
            FirstName = "Juan", LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!",
            Roles = UserTestHelper.GetValidRolesDto()
        };

        _userService.CreateUser(userDto);

        var result = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
    }

    [TestMethod]
    public void GetUserDto_WithExistingUser_ShouldReturnUserResponseDto()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        var result = _userService.GetUserDto("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
        Assert.AreEqual(new DateTime(2000, 1, 1), result.BirthDate);
    }

    [TestMethod]
    public void GetUserDto_WithNonExistingUser_ShouldReturnNull()
    {
        Assert.IsNull(_userService.GetUserDto("noone@gmail.com"));
    }

    [TestMethod]
    public void GetUserDto_WithEmptyEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => _userService.GetUserDto(""));
    }

    [TestMethod]
    public void GenerateDefaultPassword_ShouldReturnValidPassword()
    {
        var pwd = User.GenerateDefaultPassword();

        Assert.IsNotNull(pwd);
        Assert.AreEqual(10, pwd.Length);
        Assert.IsTrue(pwd.Any(char.IsUpper));
        Assert.IsTrue(pwd.Any(char.IsLower));
        Assert.IsTrue(pwd.Any(char.IsDigit));
        Assert.IsTrue(pwd.Any(ch => !char.IsLetterOrDigit(ch)));
    }

    [TestMethod]
    public void CreateUser_WithRolesDto_ShouldCreateUserWithRoles()
    {
        var userDto = new UserRequestDto
        {
            FirstName = "Juan", LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!",
            Roles = new List<UserRoleDto> { UserRoleDto.SystemAdministrator, UserRoleDto.Editor }
        };

        _userService.CreateUser(userDto);

        var result = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Roles.Count);
        Assert.IsTrue(result.Roles.Contains(UserRole.SystemAdministrator));
        Assert.IsTrue(result.Roles.Contains(UserRole.Editor));
    }

    [TestMethod]
    public void UpdateUser_WithRoles_ShouldUpdateUserRoles()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        _userService.UpdateUser("Juan", "Gomez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!",
            new List<UserRole> { UserRole.SystemAdministrator, UserRole.Editor });

        var result = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Roles.Count);
        Assert.IsTrue(result.Roles.Contains(UserRole.SystemAdministrator));
        Assert.IsTrue(result.Roles.Contains(UserRole.Editor));
    }

    [TestMethod]
    public void GetUserDto_WithRoles_ShouldReturnRoles()
    {
        _userService.CreateUser(new UserRequestDto
        {
            FirstName = "Juan", LastName = "Perez",
            Email = "juan.perez@gmail.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "Password1!",
            Roles = new List<UserRoleDto> { UserRoleDto.SystemAdministrator, UserRoleDto.Editor }
        });

        var result = _userService.GetUserDto("juan.perez@gmail.com");
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Roles.Count);
        CollectionAssert.Contains(result.Roles, UserRoleDto.SystemAdministrator);
        CollectionAssert.Contains(result.Roles, UserRoleDto.Editor);
    }

    [TestMethod]
    public void ResetPassword_WithExistingUser_ShouldUpdatePassword()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        string newPassword = _userService.ResetPassword("juan.perez@gmail.com");

        var user = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(user);
        Assert.IsTrue(user.HasPassword(newPassword));
    }

    [TestMethod]
    public void ChangeOwnPassword_WithExistingUser_ShouldUpdatePassword()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        _userService.ChangeOwnPassword("juan.perez@gmail.com", "NewPassword1!");

        var user = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(user);
        Assert.IsTrue(user.HasPassword("NewPassword1!"));
    }

    [TestMethod]
    public void UpdateUserWithoutPassword_WithExistingUser_ShouldUpdateDataAndKeepPassword()
    {
        _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

        _userService.UpdateUserWithoutPassword("Juan", "Gomez", "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            new List<UserRole> { UserRole.SystemAdministrator, UserRole.Editor });

        var user = _userService.GetUser("juan.perez@gmail.com");
        Assert.IsNotNull(user);
        Assert.AreEqual("Gomez", user.LastName);
        Assert.AreEqual(2, user.Roles.Count);
        Assert.IsTrue(user.HasPassword("Password1!"));
    }

    [TestMethod]
    public void UpdateUserWithoutPassword_WithNonExistingUser_ShouldThrowException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.UpdateUserWithoutPassword("Juan", "Gomez", "nonexistent@gmail.com",
                new DateTime(2000, 1, 1), UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void UpdateUserWithoutPassword_WithEmptyEmail_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUserWithoutPassword("Juan", "Gomez", "",
                new DateTime(2000, 1, 1), UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
public void DeleteUser_WithCurrentUserEmail_WhenDeletingOtherUser_ShouldDeleteUser()
{
    _userService.CreateUser("Admin", "User", "admin@gmail.com",
        new DateTime(1990, 1, 1), "Password1!",
        new List<UserRole> { UserRole.SystemAdministrator });

    _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
        new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

    _userService.DeleteUser("juan.perez@gmail.com", "admin@gmail.com");

    Assert.IsNull(_userService.GetUser("juan.perez@gmail.com"));
    Assert.IsNotNull(_userService.GetUser("admin@gmail.com"));
}

[TestMethod]
public void DeleteUser_WithCurrentUserEmail_WhenDeletingSelf_ShouldThrowInvalidOperationException()
{
    _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
        new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

    Assert.ThrowsException<InvalidOperationException>(() =>
        _userService.DeleteUser("juan.perez@gmail.com", "juan.perez@gmail.com"));
}

[TestMethod]
public void DeleteUser_WithCurrentUserEmail_WhenDeletingSelfDifferentCase_ShouldThrowInvalidOperationException()
{
    _userService.CreateUser("Juan", "Perez", "juan.perez@gmail.com",
        new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());

    Assert.ThrowsException<InvalidOperationException>(() =>
        _userService.DeleteUser("juan.perez@gmail.com", "JUAN.PEREZ@gmail.com"));
}

[TestMethod]
public void DeleteUser_WithCurrentUserEmail_WhenTargetEmailIsEmpty_ShouldThrowArgumentException()
{
    Assert.ThrowsException<ArgumentException>(() =>
        _userService.DeleteUser("", "admin@gmail.com"));
}

[TestMethod]
public void DeleteUser_WithCurrentUserEmail_WhenCurrentUserEmailIsEmpty_ShouldThrowArgumentException()
{
    Assert.ThrowsException<ArgumentException>(() =>
        _userService.DeleteUser("juan.perez@gmail.com", ""));
}

[TestMethod]
public void DeleteUser_WithCurrentUserEmail_WhenTargetUserDoesNotExist_ShouldThrowInvalidOperationException()
{
    _userService.CreateUser("Admin", "User", "admin@gmail.com",
        new DateTime(1990, 1, 1), "Password1!",
        new List<UserRole> { UserRole.SystemAdministrator });

    Assert.ThrowsException<InvalidOperationException>(() =>
        _userService.DeleteUser("missing@gmail.com", "admin@gmail.com"));
}

[TestMethod]
public void ResetPassword_WithNonExistingUser_ShouldThrowInvalidOperationException()
{
    Assert.ThrowsException<InvalidOperationException>(() =>
        _userService.ResetPassword("missing@gmail.com"));
}

[TestMethod]
public void ResetPassword_WithEmptyEmail_ShouldThrowArgumentException()
{
    Assert.ThrowsException<ArgumentException>(() =>
        _userService.ResetPassword(""));
}

[TestMethod]
public void ChangeOwnPassword_WithNonExistingUser_ShouldThrowInvalidOperationException()
{
    Assert.ThrowsException<InvalidOperationException>(() =>
        _userService.ChangeOwnPassword("missing@gmail.com", "NewPassword1!"));
}

[TestMethod]
public void ChangeOwnPassword_WithEmptyEmail_ShouldThrowArgumentException()
{
    Assert.ThrowsException<ArgumentException>(() =>
        _userService.ChangeOwnPassword("", "NewPassword1!"));
}

[TestMethod]
public void CanExport_WhenUserIsSystemAdministrator_ShouldReturnTrue()
{
    _userService.CreateUser("Admin", "User", "admin@gmail.com",
        new DateTime(1990, 1, 1), "Password1!",
        new List<UserRole> { UserRole.SystemAdministrator });

    Assert.IsTrue(_userService.CanExport("admin@gmail.com"));
}

[TestMethod]
public void CanExport_WhenUserIsJournalist_ShouldReturnTrue()
{
    _userService.CreateUser("Juan", "Periodista", "periodista@gmail.com",
        new DateTime(1995, 1, 1), "Password1!",
        new List<UserRole> { UserRole.Journalist });

    Assert.IsTrue(_userService.CanExport("periodista@gmail.com"));
}

[TestMethod]
public void CanExport_WhenUserIsEditor_ShouldReturnFalse()
{
    _userService.CreateUser("Editor", "User", "editor@gmail.com",
        new DateTime(1995, 1, 1), "Password1!",
        new List<UserRole> { UserRole.Editor });

    Assert.IsFalse(_userService.CanExport("editor@gmail.com"));
}

[TestMethod]
public void CanExport_WhenUserDoesNotExist_ShouldReturnFalse()
{
    Assert.IsFalse(_userService.CanExport("missing@gmail.com"));
}
}