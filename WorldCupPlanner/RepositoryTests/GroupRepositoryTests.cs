using Domain.Entities;
using Repository;

namespace RepositoryTests;

[TestClass]
public class GroupRepositoryTests
{
    [TestMethod]
    public void AddGroup_WithValidGroup_ShouldAddGroup()
    {
        var repository = new GroupRepository();
        var newGroup = new Group('A');
        
        repository.AddGroup(newGroup);
        Assert.AreEqual(1, repository.GetAllGroups().Count);
        Assert.AreEqual(newGroup, repository.GetAllGroups()[0]);
    }

    [TestMethod]
    public void AddGroup_WithNullGroup_ShouldThrowArgumentNullException()
    {
        var repository = new GroupRepository();
        Assert.ThrowsException<ArgumentNullException>(() => repository.AddGroup(null));
    }
    
    [TestMethod]
    public void GetGroup_WithExistingName_ShouldReturnGroup()
    {
        var repository = new GroupRepository();
        var groupA = new Group('A');
        repository.AddGroup(groupA);        
        var result = repository.GetGroupByName('A');
        Assert.AreEqual(groupA, result);
    }
    
    [TestMethod]
    public void GetGroup_WithNonExistingGroup_ShouldThrowKeyNotFoundException()
    {
        var repository = new GroupRepository();
        Assert.ThrowsException<KeyNotFoundException>(() => repository.GetGroupByName('A'));
    }
    
    [TestMethod]
    public void GetGroup_WithInvalidGroupLetter_ShouldThrowArgumentException()
    {
        var repository = new GroupRepository();
        Assert.ThrowsException<ArgumentException>(() => repository.GetGroupByName('0'));
    }
    
    [TestMethod]
    public void GetAllGroups_WhenRepositoryIsEmpty_ShouldReturnEmptyList()
    {
        var repository = new GroupRepository();
        var groups = repository.GetAllGroups();
        Assert.AreEqual(0, groups.Count);
    }
    
    [TestMethod]
    public void GetAllGroups_WithMultipleGroups_ShouldReturnAllGroups()
    {
        var repository = new GroupRepository();
        var groupA = new Group('A');
        var groupB = new Group('B');
        var groupC = new Group('C');

        repository.AddGroup(groupA);
        repository.AddGroup(groupB);
        repository.AddGroup(groupC);

        var allGroups = repository.GetAllGroups();
        Assert.AreEqual(3, allGroups.Count);
        Assert.IsTrue(allGroups.Contains(groupA));
        Assert.IsTrue(allGroups.Contains(groupB));
        Assert.IsTrue(allGroups.Contains(groupC));
    }
}
