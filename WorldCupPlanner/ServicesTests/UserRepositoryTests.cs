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
    
    [TestMethod]
    public void GivenNullUser_AddUser_ShouldThrowException()
    {
        var userRepository = new UserRepository();

        Assert.ThrowsException<ArgumentNullException>(() => userRepository.AddUser(null));
    }
    
    [TestMethod]
    public void GivenExistingUser_AddDuplicate_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var user1 = new User("Juan", "Perez", "juan.perez@gmail.com", new DateTime(2000, 1, 1), "Password1!");
        var user2 = new User("Pedro", "Gomez", "juan.perez@gmail.com", new DateTime(1999, 5, 10), "Password1!");

        userRepository.AddUser(user1);

        Assert.ThrowsException<ArgumentException>(() => userRepository.AddUser(user2));
    }
}