using Repository;
using RepositoryTests.TestHelpers;

namespace RepositoryTests;

[TestClass]
public class UserRepositoryTests
{
    private WorldCupPlannerContext _context = null!;
    private UserRepository _userRepository = null!;
    

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextFactory.CreateInMemoryContext();
        _userRepository = new UserRepository(_context);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
    
    [TestMethod]
    public void Add_OneUser_ShouldNotThrowException()
    {
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");
        
        _userRepository.AddUser(user);
    }
    
    [TestMethod]
    public void GivenNullUser_AddUser_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _userRepository.AddUser(null));
    }
    
    [TestMethod]
    public void GivenExistingUser_AddDuplicate_ShouldThrowException()
    {
        var user1 = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");
        var user2 = UserTestHelper.CreateValidUser("Pedro", "Gomez", "juan.perez@gmail.com", new DateTime(1999, 5, 10));
        
        _userRepository.AddUser(user1);
        
        Assert.ThrowsException<ArgumentException>(() => _userRepository.AddUser(user2));
    }
    
    [TestMethod]
    public void Get_ExistingUser_ShouldReturnUser()
    {
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");
        _userRepository.AddUser(user);

        var result = _userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("juan.perez@gmail.com", result.Email);
    }
    
    [TestMethod]
    public void Get_NonExistingUser_ShouldReturnNull()
    {
        var result = _userRepository.GetUser("juan.perez@gmail.com");
        
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void GivenExistingUser_DeleteUser_ShouldDeleteUser()
    {
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        _userRepository.AddUser(user);
        _userRepository.DeleteUser(user);

        var result = _userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void GivenNonExistingUser_DeleteUser_ShouldThrowException()
    {
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        Assert.ThrowsException<InvalidOperationException>(() => _userRepository.DeleteUser(user));
    }
    
    [TestMethod]
    public void GivenMultipleUsers_GetAllUsers_ShouldReturnUsers()
    {
        var user1 = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");
        var user2 = UserTestHelper.CreateValidUser("Pedro", "Gomez", "pedro.gomez@gmail.com", new DateTime(1999, 5, 10));

        _userRepository.AddUser(user1);
        _userRepository.AddUser(user2);

        var result = _userRepository.GetAllUsers();

        Assert.AreEqual(2, result.Count);
    }
    
    [TestMethod]
    public void GivenExistingUser_UpdateUser_ShouldUpdateUser()
    {
        var originalUser = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        _userRepository.AddUser(originalUser);

        var updatedUser = UserTestHelper.CreateValidUser("Juan", "Gomez", "juan.perez@gmail.com");

        _userRepository.UpdateUser(updatedUser);

        var result = _userRepository.GetUser("juan.perez@gmail.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Gomez", result.LastName);
    }
    
    [TestMethod]
    public void GivenNonExistingUser_UpdateUser_ShouldThrowException()
    {
        var user = UserTestHelper.CreateValidUser("Juan", "Perez", "juan.perez@gmail.com");

        Assert.ThrowsException<InvalidOperationException>(() => _userRepository.UpdateUser(user));
    }
    
    [TestMethod]
    public void GivenNullUser_UpdateUser_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _userRepository.UpdateUser(null));
    }
}