using Domain;

namespace DomainTests;

[TestClass]
public class FixtureTests
{
    [TestInitialize]
    public void Setup()
    {
        Match.ResetIdCounter();
    }
    
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
        Assert.IsFalse(result);
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
    
    [TestMethod]
    public void IsValid_WithCompleteFixture_ReturnsTrue()
    {
        var fixture = CreateValidFixture();
        var result = fixture.IsValid();
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToGroupStage_WhenGroupsExist_ReturnsTrue()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        var result = fixture.CanAdvanceToPhase(TournamentPhase.GroupStage);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToGroupStage_WhenNoGroups_ReturnsFalse()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        var result = fixture.CanAdvanceToPhase(TournamentPhase.GroupStage);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToRoundOf32_WhenGroupStageComplete_ReturnsTrue()
    {
        var fixture = CreateValidFixture();
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        foreach (var group in fixture.GetGroups)
        {
            foreach (var match in group.MatchesInGroup)
                match.SetMatchResult(1, 0);
        }
        var result = fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToRoundOf32_WhenNotInGroupStage_ReturnsFalse()
    {
        var fixture = CreateValidFixture();
        var result = fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void AdvanceToPhase_WhenValid_ChangesPhase()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        Assert.AreEqual(TournamentPhase.GroupStage, fixture.CurrentPhase);
    }
    
    [TestMethod]
    public void AdvanceToPhase_WhenInvalid_ThrowsException()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config); 
        Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.AdvanceToPhase(TournamentPhase.GroupStage));
    }
    
    [TestMethod]
    public void IsStillValid_WithAllTeamsAndStadiumsExisting_ReturnsTrue()
    {
        var fixture = CreateValidFixture();
        List<Team> teams = new List<Team>();
        List<Stadium> stadiums = new List<Stadium>();
    
        foreach (var group in fixture.GetGroups)
        {
            foreach (var team in group.TeamsInGroup)
                teams.Add((Team)team);
        
            foreach (var match in group.MatchesInGroup)
            {
                if (!stadiums.Contains((Stadium)match.Stadium))
                    stadiums.Add((Stadium)match.Stadium);
            }
        }
        bool result = fixture.IsStillValid(teams, stadiums);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsStillValid_WhenTeamIsMissing_ReturnsFalse()
    {
        var fixture = CreateValidFixture();
        List<Team> teams = new List<Team>();
        List<Stadium> stadiums = new List<Stadium>();
    
        for (int i = 1; i < fixture.GetGroups.Count; i++)
        {
            var group = fixture.GetGroups[i];
            foreach (var team in group.TeamsInGroup)
                teams.Add((Team)team);
        }
        foreach (var group in fixture.GetGroups)
        {
            foreach (var match in group.MatchesInGroup)
            {
                if (!stadiums.Contains((Stadium)match.Stadium))
                    stadiums.Add((Stadium)match.Stadium);
            }
        }
        bool result = fixture.IsStillValid(teams, stadiums);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsStillValid_WhenStadiumIsMissing_ReturnsFalse()
    {
        var fixture = CreateValidFixture();
        List<Team> teams = new List<Team>();
        List<Stadium> stadiums = new List<Stadium>();
    
        foreach (var group in fixture.GetGroups)
        {
            foreach (var team in group.TeamsInGroup)
                teams.Add((Team)team);
            foreach (var match in group.MatchesInGroup)
            {
                if (!stadiums.Contains((Stadium)match.Stadium))
                    stadiums.Add((Stadium)match.Stadium);
            }
        }
        stadiums.Clear();
        bool result = fixture.IsStillValid(teams, stadiums);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsStillValid_WhenFixtureIsEmpty_ReturnsFalse()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        List<Team> teams = new List<Team>();
        List<Stadium> stadiums = new List<Stadium>();
        bool result = fixture.IsStillValid(teams, stadiums);
        Assert.IsFalse(result);
    }
    
    private Fixture CreateValidFixture()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
    
        foreach (var group in fixture.GetGroups)
        {
            for (int i = 1; i <= 4; i++)
            {
                var team = new Team($"Team_{group.GroupLetter}_{i}", new Confederation("UEFA"), 1000);
                group.AddTeamToGroup(team);
            }
        
            var clock = new ClockStub(DateTime.Now);
            var stadium = new Stadium("Estadio", "Ciudad", "", 50000);
            var teams = group.TeamsInGroup.ToList();
        
            for (int i = 0; i < 6; i++)
            {
                var match = new Match(clock.Now, stadium, teams[0], teams[1], group.GroupLetter, clock);
                group.AddMatchToGroup(match);
            }
        }
        return fixture;
    }
}