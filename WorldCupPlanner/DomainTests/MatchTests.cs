using Domain;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class MatchTests
{
   [TestInitialize]
   public void Setup()
   {
      Match.ResetIdCounter();
   }
   
   [TestMethod]
   public void CreateMatch_WithValidName_ShouldCreateNewMatch()
   {
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
      var stadium = new StadiumStub("Centenario");
      var homeTeam = new TeamStub("Uruguay");
      var awayTeam = new TeamStub("Argentina");

      var match = new Match(clock.Now, stadium, homeTeam, awayTeam, 'A', clock);

      Assert.AreEqual(1, match.IdMatch);
      Assert.AreEqual(clock.Now, match.DateMatch);
      Assert.AreEqual(stadium, match.Stadium);
      Assert.AreEqual(homeTeam, match.HomeTeam);
      Assert.AreEqual(awayTeam, match.AwayTeam);
      Assert.AreEqual('A', match.Group);
      Assert.IsNull(match.HomeGoals);
      Assert.IsNull(match.AwayGoals);
      Assert.IsNull(match.WinnerMatch);
   }

   [TestMethod]
   public void CreateMatch_WithInvalidGroup_ShouldThrowArgumentException()
   {
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
      Assert.ThrowsException<ArgumentException>(() => new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'M', clock));
   }
   
   [TestMethod]
   public void CreateMatch_WithSameTeams_ShouldThrowArgumentException()
   {
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
      var team = new TeamStub("Uruguay");
      Assert.ThrowsException<ArgumentException>(() => new Match(clock.Now, new StadiumStub("Centenario"), team, team, 'A', clock));
   }

   
   [TestMethod]
   public void CreateMatch_WithNullStadium_ShouldThrowArgumentNullException()
   {
      var homeTeam = new TeamStub("Uruguay");
      var awayTeam = new TeamStub("Argentina");
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1, 14, 0, 0));
      Assert.ThrowsException<ArgumentNullException>(() => new Match(clock.Now, null, homeTeam, awayTeam, 'A', clock));
   }
   
   [TestMethod]
   public void CreateMatch_WithNullHomeTeam_ShouldThrowArgumentNullException()
   {
      var awayTeam = new TeamStub("Argentina");
      var stadium = new StadiumStub("Estadio Centenario");
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1, 14, 0, 0));
      Assert.ThrowsException<ArgumentNullException>(() => new Match(clock.Now, stadium, null, awayTeam, 'A', clock));
   }
   
  
   [TestMethod]
   public void CreateMatch_WithNullAwayTeam_ShouldThrowArgumentNullException()
   {
      var homeTeam = new TeamStub("Uruguay");
      var stadium = new StadiumStub("Estadio Centenario");
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1, 14, 0, 0));
      Assert.ThrowsException<ArgumentNullException>(() => new Match(clock.Now, stadium, homeTeam, null, 'A', clock));
   }
   
  [TestMethod]
  public void CreateMatch_WithNullGroup_ShouldThrowArgumentNullException()
  {
     var homeTeam = new TeamStub("Uruguay");
     var awayTeam = new TeamStub("Argentina");
     var stadium = new StadiumStub("Estadio Centenario");
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1, 14, 0, 0));
     Assert.ThrowsException<ArgumentNullException>(() => new Match(clock.Now, stadium, homeTeam, awayTeam, '\0', clock));
  }
  
  [TestMethod]
  public void SetResult_WithValidGoals_ShouldAssignGoalsAndWinner()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var home = new TeamStub("Uruguay");
     var away = new TeamStub("Argentina");
     var match = new Match(clock.Now, new StadiumStub("Centenario"), home, away, 'A', clock);
     match.SetMatchResult(2, 1);
     Assert.AreEqual(2, match.HomeGoals);
     Assert.AreEqual(1, match.AwayGoals);
     Assert.AreEqual(home, match.WinnerMatch);
  }
  
  [TestMethod]
  public void SetResult_WithNegativeGoals_ShouldThrowException()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     Assert.ThrowsException<ArgumentException>(() => match.SetMatchResult(-1, 1));
  }

  [TestMethod]
  public void SetResult_WhenAlreadySet_ShouldThrowArgumentException()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     match.SetMatchResult(1, 0);
     Assert.ThrowsException<InvalidOperationException>(() => match.SetMatchResult(2, 0));
  }

  [TestMethod]
  public void SetResult_BeforeMatchDate_ShouldThrowArgumentException()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var matchDate = new DateTime(2026, 6, 10);
     var match = new Match(matchDate, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     Assert.ThrowsException<InvalidOperationException>(() => match.SetMatchResult(1, 0));
  }

  [TestMethod]
  public void CreateMatch_WithPastDate_ShouldThrowArgumentException()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 10));
     var pastDate = new DateTime(2026, 6, 1);
     Assert.ThrowsException<ArgumentException>(() => new Match(pastDate, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock));
  }

  [TestMethod]
  public void CreateMatch_AfterResetIdCounter_ShouldStartFromOne()
  {
     Match.ResetIdCounter();
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match1 = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     var match2 = new Match(clock.Now, new StadiumStub("Maracana"), new TeamStub("Brasil"), new TeamStub("Peru"), 'A', clock);
     Assert.AreEqual(1, match1.IdMatch);
     Assert.AreEqual(2, match2.IdMatch);
  }
  
  [TestMethod]
  public void CreateMatch_WithNullClock_ShouldThrowArgumentNullException()
  {
     Assert.ThrowsException<ArgumentNullException>(() =>
        new Match( new DateTime(2026, 6, 1), new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', null));
  }
  
  [TestMethod]
  public void CreateMatch_WithLowercaseGroup_ShouldSaveGroupAsUppercase()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match( clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'a', clock);
     Assert.AreEqual('A', match.Group);
  }
  
  [TestMethod]
  public void SetMatchResult_WithAwayTeamWinner_ShouldAssignAwayTeamAsWinner()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var awayTeam = new TeamStub("Argentina");
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), awayTeam, 'A', clock);

     match.SetMatchResult(0, 2);
     Assert.AreEqual(awayTeam, match.WinnerMatch);
  }
  
  [TestMethod]
  public void SetMatchResult_WithDraw_ShouldLeaveWinnerNull()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);

     match.SetMatchResult(1, 1);
     Assert.IsNull(match.WinnerMatch);
  }
  
  [TestMethod]
  public void UpdateDate_WithValidDate_ShouldUpdateDate()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     var newDate = new DateTime(2026, 6, 5);

     match.UpdateDate(newDate);
     Assert.AreEqual(newDate, match.DateMatch);
  }
  
  [TestMethod]
  public void UpdateDate_WithPastDate_ShouldThrowArgumentException()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 10));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     var pastDate = new DateTime(2026, 6, 1);

     Assert.ThrowsException<ArgumentException>(() => match.UpdateDate(pastDate));
  }
  
  [TestMethod]
  public void UpdateStadium_WithValidStadium_ShouldUpdateStadium()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);
     var newStadium = new StadiumStub("Maracana");

     match.UpdateStadium(newStadium);
     Assert.AreEqual(newStadium, match.Stadium);
  }
  
  [TestMethod]
  public void UpdateStadium_WithNullStadium_ShouldThrowArgumentNullException()
  {
     var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
     var match = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);

     Assert.ThrowsException<ArgumentNullException>(() => match.UpdateStadium(null));
  }
}