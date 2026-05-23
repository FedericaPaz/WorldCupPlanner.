using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class WorldCupPlannerContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Stadium> Stadiums { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Cruce> Cruces { get; set; }

    public WorldCupPlannerContext(DbContextOptions<WorldCupPlannerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Email);
            entity.Property(u => u.Roles)
                .HasConversion(
                    roles => string.Join(",", roles.Select(r => r.ToString())),
                    value => value
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => Enum.Parse<UserRole>(r))
                        .ToList()
                );
        });

        modelBuilder.Entity<Team>()
            .HasKey(team => team.Name);

        modelBuilder.Entity<Stadium>()
            .HasKey(stadium => stadium.StadiumName);

        modelBuilder.Entity<LogEntry>()
            .HasKey(log => log.Id);

        modelBuilder.Entity<Notification>()
            .HasKey(notification => notification.Id);
        
        modelBuilder.Entity<Group>()
            .HasKey(group => group.Id);

        modelBuilder.Entity<Group>()
            .Property(group => group.GroupLetter)
            .IsRequired();

        modelBuilder.Entity<Group>()
            .Ignore(group => group.TeamsInGroup);

        modelBuilder.Entity<Group>()
            .Ignore(group => group.MatchesInGroup);
        
        modelBuilder.Entity<Cruce>()
            .HasKey(cruce => cruce.Id);

        modelBuilder.Entity<Cruce>()
            .Property(cruce => cruce.Name)
            .IsRequired();

        modelBuilder.Entity<Cruce>()
            .Property(cruce => cruce.Phase)
            .IsRequired();

        modelBuilder.Entity<Cruce>()
            .Ignore(cruce => cruce.Match);

        modelBuilder.Entity<Cruce>()
            .Ignore(cruce => cruce.NextCruce);
    }
}