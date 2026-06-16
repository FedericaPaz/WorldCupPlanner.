using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Repository;
using Domain.Enums;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace RepositoryTests;

[TestClass]
public class TeamRepositoryTests
{
    private WorldCupPlannerContext _context;
    private TeamRepository _teamRepository;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);
        _teamRepository = new TeamRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AddTeam_ShouldStoreTeam()
    {
        var team = CreateTeam("Uruguay");

        _teamRepository.Add(team);

        var teams = _teamRepository.GetAll();

        Assert.AreEqual(1, teams.Count);
        Assert.AreEqual(team.Name, teams[0].Name);
    }

    [TestMethod]
    public void AddTeam_Null_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _teamRepository.Add(null));
    }

    [TestMethod]
    public void Get_ExistingTeam_ShouldReturnTeam()
    {
        var team = CreateTeam("Uruguay");

        _teamRepository.Add(team);

        var result = _teamRepository.Get("Uruguay");

        Assert.IsNotNull(result);
        Assert.AreEqual(team.Name, result.Name);
    }

    [TestMethod]
    public void Get_NonExistingTeam_ShouldReturnNull()
    {
        var result = _teamRepository.Get("Argentina");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GivenMultipleTeams_WhenGetAll_ShouldReturnAllTeams()
    {
        var team1 = CreateTeam("Uruguay");
        var team2 = CreateTeam("Argentina");

        _teamRepository.Add(team1);
        _teamRepository.Add(team2);

        var teams = _teamRepository.GetAll();

        Assert.AreEqual(2, teams.Count);
        Assert.IsTrue(teams.Any(t => t.Name == "Uruguay"));
        Assert.IsTrue(teams.Any(t => t.Name == "Argentina"));
    }

    [TestMethod]
    public void DeleteTeam_WhenExists_ShouldRemoveTeam()
    {
        var team = CreateTeam("Uruguay");

        _teamRepository.Add(team);
        _teamRepository.Delete(team);

        Assert.IsNull(_teamRepository.Get("Uruguay"));
    }

    [TestMethod]
    public void DeleteTeam_WhenNonExists_ShouldThrowException()
    {
        var team = CreateTeam("Uruguay");

        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Delete(team));
    }
    
    [TestMethod]
    public void DeleteTeam_Null_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _teamRepository.Delete(null!));
    }

    [TestMethod]
    public void UpdateTeam_WhenExists_ShouldUpdateTeam()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var updatedTeam = new Team("Uruguay", new Confederation("CONMEBOL"), 1000);
        _teamRepository.Update(updatedTeam);

        var result = _teamRepository.Get("Uruguay");

        Assert.AreEqual("CONMEBOL", result.Confederation.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }

    [TestMethod]
    public void UpdateTeam_WhenNonExists_ShouldThrowException()
    {
        var team = CreateTeam("Uruguay");

        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Update(team));
    }
    
    [TestMethod]
    public void UpdateTeam_Null_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _teamRepository.Update(null!));
    }

    [TestMethod]
    public void GetAll_ShouldReturnCopy_NotInternalList()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var list = _teamRepository.GetAll();
        list.Clear();

        var result = _teamRepository.GetAll();

        Assert.AreEqual(1, result.Count);
    }

    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("AFC"), 500);
    }
    
    [TestMethod]
    public void Clear_ShouldRemoveAllTeams()
    {
        _teamRepository.Add(CreateTeam("Uruguay"));
        _teamRepository.Add(CreateTeam("Argentina"));

        _teamRepository.Clear();

        Assert.AreEqual(0, _teamRepository.GetAll().Count);
    }
    
    [TestMethod]
    public void UpdateTeam_WhenFlagChanges_ShouldPersistNewFlag()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1000, flagBase64: "bandera_vieja");
        _teamRepository.Add(team);
        
        var updatedTeam = new Team("Uruguay", new Confederation("CONMEBOL"), 1000, flagBase64: "bandera_nueva");
        
        _teamRepository.Update(updatedTeam);
        
        var result = _teamRepository.Get("Uruguay");

        Assert.IsNotNull(result);
        Assert.AreEqual("bandera_nueva", result.FlagBase64); 
    }

    [TestMethod]
    public void Rename_OldNameEmpty_ShouldThrowArgumentException()
    {
        var team = CreateTeam("Old");
        Assert.ThrowsException<ArgumentException>(() => _teamRepository.Rename("", team));
    }

    [TestMethod]
    public void Rename_NewTeamNull_ShouldThrowArgumentNullException()
    {
        var old = CreateTeam("Old");
        _teamRepository.Add(old);
        Assert.ThrowsException<ArgumentNullException>(() => _teamRepository.Rename("Old", null!));
    }

    [TestMethod]
    public void Rename_NonExistingOldTeam_ShouldThrowInvalidOperationException()
    {
        var newTeam = CreateTeam("New");
        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Rename("NoExiste", newTeam));
    }

    [TestMethod]
    public void Rename_ShouldAddNewRemoveOldAndUpdateIncidents()
    {
        // Arrange
        var old = CreateTeam("OldTeam");
        var other = CreateTeam("OtherTeam");
        _teamRepository.Add(old);
        _teamRepository.Add(other);

        var stadium = new Stadium("Estadio", "Ciudad", null, 20000);
        var clock = new TestClock(DateTime.UtcNow);
        var match = new Match(1, clock.Now.AddDays(1), stadium, old, other, 'A', clock);

        _context.Matches.Add(match);
        var incident = new MatchIncident(IncidentType.YellowCard, old, 1, match.IdMatch);
        _context.MatchIncidents.Add(incident);
        _context.SaveChanges();

        var newTeam = CreateTeam("NewTeam");

        // Act
        _teamRepository.Rename("OldTeam", newTeam);

        // Assert
        var gotNew = _teamRepository.Get("NewTeam");
        var gotOld = _teamRepository.Get("OldTeam");

        Assert.IsNotNull(gotNew);
        Assert.IsNull(gotOld);

        var inc = _context.MatchIncidents.AsNoTracking().FirstOrDefault();
        Assert.IsNotNull(inc);
        Assert.AreEqual("NewTeam", inc.TeamName);
    }

    [TestMethod]
    public void Rename_WhenOldIsHomeTeam_ShouldUpdateHomeTeamReference()
    {
        var old = CreateTeam("OldH");
        var other = CreateTeam("OtherH");
        _teamRepository.Add(old);
        _teamRepository.Add(other);

        var stadium = new Stadium("EstadioH", "Ciudad", null, 20000);
        var clock = new TestClock(DateTime.UtcNow);
        var match = new Match(10, clock.Now.AddDays(1), stadium, old, other, 'A', clock);

        _context.Matches.Add(match);
        _context.SaveChanges();

        var newTeam = CreateTeam("NewH");

        _teamRepository.Rename("OldH", newTeam);

        var updatedMatch = _context.Matches.Include(m => m.HomeTeam).First(m => m.IdMatch == match.IdMatch);
        Assert.AreEqual("NewH", updatedMatch.HomeTeam.Name);
        Assert.IsNull(_teamRepository.Get("OldH"));
        Assert.IsNotNull(_teamRepository.Get("NewH"));
    }

    [TestMethod]
    public void Rename_WhenOldIsAwayTeam_ShouldUpdateAwayTeamReference()
    {
        var old = CreateTeam("OldA");
        var other = CreateTeam("OtherA");
        _teamRepository.Add(old);
        _teamRepository.Add(other);

        var stadium = new Stadium("EstadioA", "Ciudad", null, 20000);
        var clock = new TestClock(DateTime.UtcNow);
        var match = new Match(11, clock.Now.AddDays(1), stadium, other, old, 'B', clock);

        _context.Matches.Add(match);
        _context.SaveChanges();

        var newTeam = CreateTeam("NewA");

        _teamRepository.Rename("OldA", newTeam);

        var updatedMatch = _context.Matches.Include(m => m.AwayTeam).First(m => m.IdMatch == match.IdMatch);
        Assert.AreEqual("NewA", updatedMatch.AwayTeam.Name);
        Assert.IsNull(_teamRepository.Get("OldA"));
        Assert.IsNotNull(_teamRepository.Get("NewA"));
    }

    [TestMethod]
    public void Rename_WhenOldIsWinnerMatch_ShouldUpdateWinnerMatchReference()
    {
        var old = CreateTeam("OldW");
        var other = CreateTeam("OtherW");
        _teamRepository.Add(old);
        _teamRepository.Add(other);

        var stadium = new Stadium("EstadioW", "Ciudad", null, 20000);
        var clock = new DomainTests.Stubs.DateTimeMatchStub(new DateTime(2099, 1, 1));

        // Create Match instance using private constructor and reflection to set fields so we can control WinnerMatch
        var match = (Match)Activator.CreateInstance(typeof(Match), true)!;
        typeof(Match).GetField("_idMatch", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, 12);
        typeof(Match).GetField("_dateMatch", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, clock.Now.AddDays(2));
        typeof(Match).GetField("_stadium", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, stadium);
        typeof(Match).GetField("_homeTeam", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, old);
        typeof(Match).GetField("_awayTeam", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, other);
        typeof(Match).GetField("_group", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, 'C');
        typeof(Match).GetField("_clock", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, clock);
        // set winner directly
        typeof(Match).GetField("_winnerMatch", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, old);

        _context.Matches.Add(match);
        _context.SaveChanges();

        var newTeam = CreateTeam("NewW");

        _teamRepository.Rename("OldW", newTeam);

        var updatedMatch = _context.Matches.Include(m => m.WinnerMatch).First(m => m.IdMatch == 12);
        Assert.IsNotNull(updatedMatch.WinnerMatch);
        Assert.AreEqual("NewW", updatedMatch.WinnerMatch.Name);
        Assert.IsNull(_teamRepository.Get("OldW"));
        Assert.IsNotNull(_teamRepository.Get("NewW"));
    }

    [TestMethod]
    public void Rename_WhenOldIsLoserMatch_ShouldUpdateLoserMatchReference()
    {
        var old = CreateTeam("OldL");
        var other = CreateTeam("OtherL");
        _teamRepository.Add(old);
        _teamRepository.Add(other);

        var stadium = new Stadium("EstadioL", "Ciudad", null, 20000);
        var clock = new DomainTests.Stubs.DateTimeMatchStub(new DateTime(2099, 1, 1));

        // create match via private ctor and set fields; set loser directly
        var match = (Match)Activator.CreateInstance(typeof(Match), true)!;
        typeof(Match).GetField("_idMatch", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, 13);
        typeof(Match).GetField("_dateMatch", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, clock.Now.AddDays(2));
        typeof(Match).GetField("_stadium", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, stadium);
        typeof(Match).GetField("_homeTeam", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, other);
        typeof(Match).GetField("_awayTeam", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, old);
        typeof(Match).GetField("_group", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, 'D');
        typeof(Match).GetField("_clock", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, clock);
        // set loser directly
        typeof(Match).GetField("_loserMatch", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(match, old);

        _context.Matches.Add(match);
        _context.SaveChanges();

        var newTeam = CreateTeam("NewL");

        _teamRepository.Rename("OldL", newTeam);

        var updatedMatch = _context.Matches.Include(m => m.LoserMatch).First(m => m.IdMatch == 13);
        Assert.IsNotNull(updatedMatch.LoserMatch);
        Assert.AreEqual("NewL", updatedMatch.LoserMatch.Name);
        Assert.IsNull(_teamRepository.Get("OldL"));
        Assert.IsNotNull(_teamRepository.Get("NewL"));
    }

    [TestMethod]
    public void Rename_WhenMatchIncidentsExist_ShouldUpdateIncidentTeamNames()
    {
        var old = CreateTeam("OldI");
        var other = CreateTeam("OtherI");
        _teamRepository.Add(old);
        _teamRepository.Add(other);

        var stadium = new Stadium("EstadioI", "Ciudad", null, 20000);
        var clock = new TestClock(DateTime.UtcNow);
        var match = new Match(14, clock.Now.AddDays(1), stadium, old, other, 'E', clock);

        _context.Matches.Add(match);
        var incident = new MatchIncident(IncidentType.RedCard, old, 1, match.IdMatch);
        _context.MatchIncidents.Add(incident);
        _context.SaveChanges();

        var newTeam = CreateTeam("NewI");

        _teamRepository.Rename("OldI", newTeam);

        var updatedIncident = _context.MatchIncidents.AsNoTracking().First(i => i.Id == incident.Id);
        Assert.AreEqual("NewI", updatedIncident.TeamName);
        Assert.IsNull(_teamRepository.Get("OldI"));
        Assert.IsNotNull(_teamRepository.Get("NewI"));
    }

    private class TestClock : Domain.Interfaces.IDateTimeMatch
    {
        public TestClock(DateTime now) => Now = now;
        public DateTime Now { get; }
    }

    [TestMethod]
    public void Rename_WithSqliteProvider_ShouldUseTransactionAndRenameTeam()
    {
        // Arrange: create SQLite in-memory connection and options
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new WorldCupPlannerContext(options);
        context.Database.EnsureCreated();

        var repo = new TeamRepository(context);

        var old = CreateTeam("SqlOld");
        repo.Add(old);

        var newTeam = CreateTeam("SqlNew");

        // Act & Assert: rename should not throw and should persist using transaction path
        repo.Rename("SqlOld", newTeam);

        Assert.IsNull(repo.Get("SqlOld"));
        Assert.IsNotNull(repo.Get("SqlNew"));
    }
}
