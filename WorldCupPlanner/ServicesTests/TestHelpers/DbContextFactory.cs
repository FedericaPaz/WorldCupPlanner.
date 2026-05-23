using Microsoft.EntityFrameworkCore;
using Repository;

namespace ServicesTests.TestHelpers;

public static class DbContextFactory
{
    public static WorldCupPlannerContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new WorldCupPlannerContext(options);
    }
}