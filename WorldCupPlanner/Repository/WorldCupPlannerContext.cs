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
    public DbSet<Match> Matches { get; set; }

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
        
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(match => match.IdMatch);

            entity.Ignore(match => match.Stadium);
            entity.Ignore(match => match.HomeTeam);
            entity.Ignore(match => match.AwayTeam);
            entity.Ignore(match => match.WinnerMatch);
            entity.Ignore(match => match.LoserMatch);
            entity.Ignore(match => match.Incidents);
        });
    }
}