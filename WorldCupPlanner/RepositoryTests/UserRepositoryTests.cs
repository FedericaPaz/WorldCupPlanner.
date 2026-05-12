using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTests.TestHelpers;

namespace RepositoryTests;

[TestClass]
public class UserRepositoryTests
{
    [TestMethod]
    public void Add_OneUser_ShouldNotThrowException()
    {
        var userRepository = new UserRepository();
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

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
        var user1 = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");
        var user2 = UserTestHelper.CreateValidUser("Pedro", "Gomez", "juan.perez@gmail.com", new DateTime(1999, 5, 10));

        userRepository.AddUser(user1);

        Assert.ThrowsException<ArgumentException>(() => userRepository.AddUser(user2));
    }
    
    [TestMethod]
    public void Get_ExistingUser_ShouldReturnUser()
    {
        var userRepository = new UserRepository();
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        userRepository.AddUser(user);

        var result = userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
    }
    
    [TestMethod]
    public void Get_NonExistingUser_ShouldReturnNull()
    {
        var userRepository = new UserRepository();

        var result = userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void GivenExistingUser_DeleteUser_ShouldDeleteUser()
    {
        var userRepository = new UserRepository();
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        userRepository.AddUser(user);
        userRepository.DeleteUser(user);

        var result = userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void GivenNonExistingUser_DeleteUser_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        Assert.ThrowsException<InvalidOperationException>(() => userRepository.DeleteUser(user));
    }
    
    [TestMethod]
    public void GivenMultipleUsers_GetAllUsers_ShouldReturnUsers()
    {
        var userRepository = new UserRepository();
        var user1 = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");
        var user2 = UserTestHelper.CreateValidUser("Pedro", "Gomez", "pedro.gomez@gmail.com", new DateTime(1999, 5, 10));

        userRepository.AddUser(user1);
        userRepository.AddUser(user2);

        var result = userRepository.GetAllUsers();

        Assert.AreEqual(2, result.Count);
    }
    
    [TestMethod]
    public void GivenExistingUser_UpdateUser_ShouldUpdateUser()
    {
        var userRepository = new UserRepository();
        var originalUser = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        userRepository.AddUser(originalUser);

        var updatedUser = UserTestHelper.CreateValidUser("Juan", "Gomez", "juan.perez@gmail.com");

        userRepository.UpdateUser(updatedUser);

        var result = userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Gomez", result.LastName);
    }
    
    [TestMethod]
    public void GivenNonExistingUser_UpdateUser_ShouldThrowException()
    {
        var userRepository = new UserRepository();
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        Assert.ThrowsException<InvalidOperationException>(() => userRepository.UpdateUser(user));
    }
    
    [TestMethod]
    public void GivenNullUser_UpdateUser_ShouldThrowException()
    {
        var userRepository = new UserRepository();

        Assert.ThrowsException<ArgumentNullException>(() => userRepository.UpdateUser(null));
    }
}