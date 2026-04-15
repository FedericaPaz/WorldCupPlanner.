using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void CreateUser_WithValidData_ShouldSucceed()
    {
        string firstName = "Juan";
        string lastName = "Perez";
        string email = "juan.perez@gmail.com";
        DateTime birthDate = new DateTime(2000, 1, 1);
        string password = "Password1!";

        User user = new User(firstName, lastName, email, birthDate, password);

        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);
        Assert.AreEqual(birthDate, user.BirthDate);
    }
}