using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Repository;

public class WorldCupPlannerContextFactory : IDesignTimeDbContextFactory<WorldCupPlannerContext>
{
    public WorldCupPlannerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorldCupPlannerContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=WorldCupPlannerDb;User Id=sa;Password=P4ssword;TrustServerCertificate=True;");

        return new WorldCupPlannerContext(optionsBuilder.Options);
    }
}