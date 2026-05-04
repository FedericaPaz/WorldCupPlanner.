using Domain;

namespace DomainTests;

[TestClass]
public class GroupTests
{
    [TestMethod]
    public void CreateGroup_WithValidLetter_ShouldCreateGroup()
    {
        var group = new Group('A');
        Assert.AreEqual('A', group.GroupLetter);
    }
}