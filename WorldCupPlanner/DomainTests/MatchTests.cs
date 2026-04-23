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
      var dateMatch = new DateTimeMatchStub(new DateTime(2026, 6, 1, 14, 0, 0));

      var match = new Match(dateMatch.Now, stadium, homeTeam, awayTeam, 'A');

      Assert.IsNotNull(match);
      Assert.AreEqual(1, match.IdMatch);
      Assert.AreEqual(dateMatch.Now, match.DateMatch);
      Assert.AreEqual(stadium, match.Stadium);
      Assert.AreEqual(homeTeam, match.HomeTeam);
      Assert.AreEqual(awayTeam, match.AwayTeam);
      Assert.AreEqual('A', match.Group);
      Assert.IsNull(match.HomeGoals);
      Assert.IsNull(match.AwayGoals);
      Assert.IsNull(match.WinnerMatch);
   }
}