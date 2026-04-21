using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        User user = new User(firstName, lastName, email, birthDate, password);

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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
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
            new User(firstName, lastName, email, birthDate, password));
    }
    
    [TestMethod]
    public void CreateUser_ShouldNotStorePlainPassword()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        User user = new User(firstName, lastName, email, birthDate, password);

        Assert.AreNotEqual(password, user.PasswordHash);
    }
    
    [TestMethod]
    public void CreateUser_ShouldStoreHashedPassword()
    {
        var firstName = "Juan";
        var lastName = "Perez";
        var email = "juan.perez@gmail.com";
        var birthDate = new DateTime(2000, 1, 1);
        var password = "Password1!";

        User user = new User(firstName, lastName, email, birthDate, password);

        Assert.AreNotEqual(password, user.PasswordHash);
    }
}