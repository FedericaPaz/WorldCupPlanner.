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
    
    [TestMethod]
    public void AdvanceToPhase_ShouldChangeCurrentPhase()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        Assert.AreEqual(TournamentPhase.GroupStage, fixture.CurrentPhase);
    }
    
    [TestMethod]
    public void InitializeGroups_WhenCalledTwice_ShouldResetAndRecreateGroups()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        var firstGroupReference = fixture.GetGroups;
        fixture.InitializeGroups();
            
        Assert.AreEqual(12, fixture.GetGroups.Count);
        Assert.AreEqual('A', fixture.GetGroups[0].GroupLetter);
        Assert.AreNotSame(firstGroupReference, fixture.GetGroups);
    }
    
    [TestMethod]
    public void IsValid_WithValidFixture_ReturnsTrue()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        var result = fixture.IsValid();
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValid_WithWrongNumberOfGroups_ReturnsFalse()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        var result = fixture.IsValid();
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValid_WithGroupMissingTeams_ReturnsFalse()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        var result = fixture.IsValid();
        Assert.IsFalse(result);
    }
}