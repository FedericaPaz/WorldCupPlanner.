using Domain;
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