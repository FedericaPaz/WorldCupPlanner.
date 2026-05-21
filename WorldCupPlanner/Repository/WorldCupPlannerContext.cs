using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class WorldCupPlannerContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Stadium> Stadiums { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public WorldCupPlannerContext(DbContextOptions<WorldCupPlannerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(user => user.Email);

        modelBuilder.Entity<Team>()
            .HasKey(team => team.Name);

        modelBuilder.Entity<Stadium>()
            .HasKey(stadium => stadium.StadiumName);

        modelBuilder.Entity<LogEntry>()
            .HasKey(log => log.Id);

        modelBuilder.Entity<Notification>()
            .HasKey(notification => notification.Id);
    }
}