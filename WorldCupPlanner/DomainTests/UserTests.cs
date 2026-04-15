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
}