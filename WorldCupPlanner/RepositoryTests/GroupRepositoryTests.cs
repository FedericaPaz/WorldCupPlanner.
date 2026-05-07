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
}