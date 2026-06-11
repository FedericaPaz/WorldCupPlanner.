using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryTests;

[TestClass]
public class GroupRepositoryTests
{
    private WorldCupPlannerContext _context = null!;
    private GroupRepository _groupRepository = null!;
    private Group _group = null!;
    private Group _group2 = null!;
    private Group _group3 = null!;


    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new WorldCupPlannerContext(options);
        _groupRepository = new GroupRepository(_context);
        _group = new Group('A');
        _group2 = new Group('B');
        _group3 = new Group('C');
    }
    
    [TestMethod]
    public void AddGroup_WithValidGroup_ShouldAddGroup()
    {
        _groupRepository.AddGroup(_group);
        Assert.AreEqual(1, _groupRepository.GetAllGroups().Count);
        Assert.AreEqual(_group, _groupRepository.GetAllGroups()[0]);
    }

    [TestMethod]
    public void AddGroup_WithNullGroup_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _groupRepository.AddGroup(null));
    }
    
    [TestMethod]
    public void GetGroup_WithExistingName_ShouldReturnGroup()
    { ;
        _groupRepository.AddGroup(_group);        
        var result = _groupRepository.GetGroupByName('A');
        Assert.AreEqual(_group, result);
    }
    
    [TestMethod]
    public void GetGroup_WithNonExistingGroup_ShouldThrowKeyNotFoundException()
    {
        Assert.ThrowsException<KeyNotFoundException>(() => _groupRepository.GetGroupByName('A'));
    }
    
    [TestMethod]
    public void GetGroup_WithInvalidGroupLetter_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => _groupRepository.GetGroupByName('0'));
    }
    
    [TestMethod]
    public void GetAllGroups_WhenRepositoryIsEmpty_ShouldReturnEmptyList()
    {
        var groups = _groupRepository.GetAllGroups();
        Assert.AreEqual(0, groups.Count);
    }
    
    [TestMethod]
    public void GetAllGroups_WithMultipleGroups_ShouldReturnAllGroups()
    {

        _groupRepository.AddGroup(_group);
        _groupRepository.AddGroup(_group2);
        _groupRepository.AddGroup(_group3);

        var allGroups = _groupRepository.GetAllGroups();
        Assert.AreEqual(3, allGroups.Count);
        Assert.IsTrue(allGroups.Contains(_group));
        Assert.IsTrue(allGroups.Contains(_group2));
        Assert.IsTrue(allGroups.Contains(_group3));
    }
}
