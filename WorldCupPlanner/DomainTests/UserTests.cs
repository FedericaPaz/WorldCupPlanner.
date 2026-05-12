using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainTests.TestHelpers;
using Domain;

namespace DomainTests;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void CreateUser_WithValidData_ShouldSucceed()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        User user = new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles());

        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);
        Assert.AreEqual(birthDate, user.BirthDate);
    }
    
    [TestMethod]
    public void CreateUser_WithEmptyFirstName_ShouldFail()
    {
        var firstName = "";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void CreateUser_WithEmptyLastName_ShouldFail()
    {
        var firstName = "Juan";
        var lastName = "";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void CreateUser_WithEmptyEmail_ShouldFail()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }

    [TestMethod]
    public void CreateUser_WithFutureBirthDate_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = DateTime.Today.AddDays(1);
        var password = "Password1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
   
    [TestMethod]
    public void CreateUser_WithNullPassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        string password = null!;

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithEmptyPassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithShortPassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Ab1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithoutUppercasePassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "password1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithoutLowercasePassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "PASSWORD1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithoutNumberPassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithoutSpecialCharacterPassword_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_WithInvalidEmail_ShouldThrowException()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        Assert.ThrowsException<ArgumentException>(() =>
            new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles()));
    }
    
    [TestMethod]
    public void CreateUser_ShouldStoreHashedPassword()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        User user = new User(firstName, lastName, email, birthDate, password, UserTestHelper.GetValidRoles());

        Assert.AreNotEqual(password, user.PasswordHash);
    }
    
    [TestMethod]
    public void CreateUser_WithRoles_ShouldStoreRoles()
    {
        var roles = new List<UserRole>
        {
            UserRole.SystemAdministrator,
            UserRole.Editor
        };

        var user = new User(
            "Juan",
            "Perez",
            "juan.perez@gmail.com",
            new DateTime(2000, 1, 1),
            "Password1!",
            roles
        );

        Assert.AreEqual(2, user.Roles.Count);
        CollectionAssert.Contains(user.Roles, UserRole.SystemAdministrator);
        CollectionAssert.Contains(user.Roles, UserRole.Editor);
    }
    
    [TestMethod]
    public void CreateUser_WithoutRoles_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(
                "Juan",
                "Perez",
                "juan.perez@gmail.com",
                new DateTime(2000, 1, 1),
                "Password1!",
                new List<UserRole>()
            ));
    }
    
    [TestMethod]
    public void CreateUser_WithDuplicatedRoles_ShouldThrowException()
    {
        var roles = new List<UserRole>
        {
            UserRole.Editor,
            UserRole.Editor
        };

        Assert.ThrowsException<ArgumentException>(() =>
            new User(
                "Juan",
                "Perez",
                "juan.perez@gmail.com",
                new DateTime(2000, 1, 1),
                "Password1!",
                roles
            )
        );
    }
    
    [TestMethod]
    public void Constructor_WithoutRoles_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(
                "Juan",
                "Pérez",
                "juan@test.com",
                new DateTime(2000, 1, 1),
                "Password1!",
                new List<UserRole>()
            ));
    }
    
    [TestMethod]
    public void Constructor_WithNullRoles_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new User(
                "Juan",
                "Pérez", 
                "juan@test.com",
                new DateTime(2000, 1, 1),
                "Password1!",
                null
            ));
    }

    [TestMethod]
    public void ChangePassword_WithValidPassword_ShouldUpdatePassword()
    {
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", new List<UserRole>{UserRole.SystemAdministrator});
        user.ChangePassword("NewPassword1!");
        Assert.IsTrue(user.HasPassword("NewPassword1!"));
    }

    [TestMethod]
    public void ChangePassword_WithInvalidPassword_ShouldThrowException()
    {
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", new List<UserRole>{UserRole.SystemAdministrator});
        Assert.ThrowsException<ArgumentException>(() => user.ChangePassword("123"));
    }

    [TestMethod]
    public void UpdateData_WithValidData_ShouldUpdateUserData()
    {
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        user.UpdateData("Pedro", "Gomez", new DateTime(1995, 5, 5), new List<UserRole>{UserRole.Editor});
        Assert.AreEqual("Pedro", user.FirstName);
        Assert.AreEqual("Gomez", user.LastName);
        Assert.AreEqual(new DateTime(1995, 5, 5), user.BirthDate);
        Assert.AreEqual(1, user.Roles.Count);
    }

    [TestMethod]
    public void UpdateData_ShouldKeepPassword()
    {
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        user.UpdateData("Pedro", "Gomez", new DateTime(1995, 5, 5), new List<UserRole>{UserRole.Editor});
        Assert.IsTrue(user.HasPassword("Password1!"));
    }

    [TestMethod]
    public void UpdateData_WithInvalidBirthDate_ShouldThrowException()
    {
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        Assert.ThrowsException<ArgumentException>(() => user.UpdateData("Pedro", "Gomez", DateTime.Today.AddDays(1), new List<UserRole>{UserRole.Editor}));
    }

    [TestMethod]
    public void UpdateData_WithEmptyRoles_ShouldThrowException()
    {
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!", UserTestHelper.GetValidRoles());
        Assert.ThrowsException<ArgumentException>(() => user.UpdateData("Pedro", "Gomez", new DateTime(1995, 5, 5), new List<UserRole>()));
    }
}
