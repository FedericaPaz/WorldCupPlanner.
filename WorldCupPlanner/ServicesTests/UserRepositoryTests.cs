using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;

namespace ServicesTests;

[TestClass]
public class UserRepositoryTests
{
    [TestMethod]
    public void Add_OneUser_ShouldNotThrowException()
    {
        var userRepository = new UserRepository();
        var user = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!");

        userRepository.AddUser(user);
    }
}