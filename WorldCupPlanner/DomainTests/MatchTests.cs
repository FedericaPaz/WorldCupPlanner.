using Domain;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class MatchTests
{
   [TestMethod]
   public void CreateMatch_WithValidName_ShouldCreateNewMatch()
   {
      var homeTeam = new TeamStub("Uruguay");
      var awayTeam = new TeamStub("Argentina");
      var stadium = new StadiumStub("Estadio Centenario");
      var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));

      var match = new Match(clock.Now, stadium, homeTeam, awayTeam, 'A', clock);

      Assert.AreEqual(1, match.IdMatch);
      Assert.AreEqual(homeTeam, match.HomeTeam);
      Assert.AreEqual(awayTeam, match.AwayTeam);
      Assert.AreEqual('A', match.Group);
      Assert.IsNull(match.HomeGoals);
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
}