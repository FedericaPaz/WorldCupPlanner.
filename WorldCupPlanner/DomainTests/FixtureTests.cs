using System.Diagnostics.Tracing;
using Domain;

namespace DomainTests;

[TestClass]
public class FixtureTests
{
    [TestMethod]
    public void Constructor_SetPhaseToNotSarted()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        Assert.AreEqual(TournamentPhase.NotStarted, fixture.CurrentPhase);
    }
    
    [TestMethod]
    public void InitializeGroups_ShouldCreate12Groups()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
            
        Assert.AreEqual(12, fixture.GetGroups.Count);
        Assert.AreEqual('A', fixture.GetGroups[0].GroupLetter);
        Assert.AreEqual('L', fixture.GetGroups[11].GroupLetter);
            
        var expectedLetters = Enumerable.Range('A', 12).Select(c => (char)c).ToList();
        var actualLetters = fixture.GetGroups.Select(g => g.GroupLetter).ToList();
        CollectionAssert.AreEqual(expectedLetters, actualLetters);
    }
}