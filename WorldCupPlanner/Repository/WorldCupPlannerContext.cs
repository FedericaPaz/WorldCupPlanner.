using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
    public DbSet<Match> Matches { get; set; }
    public DbSet<MatchIncident> MatchIncidents { get; set; }

    public WorldCupPlannerContext(DbContextOptions<WorldCupPlannerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var rolesComparer = new ValueComparer<IReadOnlyList<UserRole>>(
            (roles1, roles2) => roles1 != null && roles2 != null && roles1.SequenceEqual(roles2),
            roles => roles.Aggregate(0, (hash, role) => HashCode.Combine(hash, role.GetHashCode())),
            roles => roles.ToList()
        );

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Email);

            entity.Property(user => user.Roles)
                .HasConversion(
                    roles => string.Join(",", roles.Select(role => role.ToString())),
                    value => value
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(role => Enum.Parse<UserRole>(role))
                        .ToList()
                )
                .Metadata.SetValueComparer(rolesComparer);
        });

        modelBuilder.Entity<Team>()
            .HasKey(team => team.Name);

        modelBuilder.Entity<Stadium>()
            .HasKey(stadium => stadium.StadiumName);

        modelBuilder.Entity<LogEntry>()
            .HasKey(log => log.Id);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey("_id");

            entity.Property<Guid>("_id")
                .HasColumnName("Id");

            entity.Property<string>("_recipientEmail")
                .HasColumnName("RecipientEmail")
                .IsRequired();

            entity.Property<string>("_message")
                .HasColumnName("Message")
                .IsRequired();

            entity.Property<DateTime>("_timestamp")
                .HasColumnName("Timestamp")
                .IsRequired();

            entity.Property<bool>("_isRead")
                .HasColumnName("IsRead")
                .IsRequired();

            entity.Property<DateTime?>("_readAt")
                .HasColumnName("ReadAt");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(group => group.Id);

            entity.Property(group => group.GroupLetter)
                .IsRequired();

            entity.Ignore(group => group.TeamsInGroup);
            entity.Ignore(group => group.MatchesInGroup);
        });

        modelBuilder.Entity<Cruce>(entity =>
        {
            entity.HasKey(cruce => cruce.Id);

            entity.Property(cruce => cruce.Name)
                .IsRequired();

            entity.Property(cruce => cruce.Phase)
                .IsRequired();

            entity.Ignore(cruce => cruce.Match);
            entity.Ignore(cruce => cruce.NextCruce);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(match => match.IdMatch);

            entity.HasOne(match => match.Stadium)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(match => match.HomeTeam)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(match => match.AwayTeam)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(match => match.WinnerMatch)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(match => match.LoserMatch)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(match => match.Incidents)
                .WithOne(incident => incident.Match)
                .HasForeignKey(incident => incident.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchIncident>(entity =>
        {
            entity.HasKey(incident => incident.Id);

            entity.Property(incident => incident.Type)
                .HasConversion<string>();

            entity.Property(incident => incident.Quantity);

            entity.HasOne(incident => incident.Team)
                .WithMany()
                .HasForeignKey(incident => incident.TeamName)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}