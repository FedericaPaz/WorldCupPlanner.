using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryTests;

[TestClass]
public class EfGroupRepositoryTests
{
    [TestMethod]
    public void AddGroup_WithValidGroup_ShouldPersistGroup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new WorldCupPlannerContext(options);
        var repository = new EfGroupRepository(context);

        var group = new Group('A');

        repository.AddGroup(group);

        var result = repository.GetGroupByName('A');

        Assert.AreEqual('A', result.GroupLetter);
    }
}