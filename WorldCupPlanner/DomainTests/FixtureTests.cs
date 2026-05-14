using Domain;
using ServicesTests.Stubs;

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
            
        Assert.AreEqual(12, fixture.Groups.Count);
        Assert.AreEqual('A', fixture.Groups[0].GroupLetter);
        Assert.AreEqual('L', fixture.Groups[11].GroupLetter);
            
        var expectedLetters = Enumerable.Range('A', 12).Select(c => (char)c).ToList();
        var actualLetters = fixture.Groups.Select(g => g.GroupLetter).ToList();
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
        var firstGroupReference = fixture.Groups;
        fixture.InitializeGroups();
            
        Assert.AreEqual(12, fixture.Groups.Count);
        Assert.AreEqual('A', fixture.Groups[0].GroupLetter);
        Assert.AreNotSame(firstGroupReference, fixture.Groups);
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
        foreach (var group in fixture.Groups)
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
    
        foreach (var group in fixture.Groups)
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
    
        for (int i = 1; i < fixture.Groups.Count; i++)
        {
            var group = fixture.Groups[i];
            foreach (var team in group.TeamsInGroup)
                teams.Add((Team)team);
        }
        foreach (var group in fixture.Groups)
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
    
        foreach (var group in fixture.Groups)
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
    
    [TestMethod]
    public void IsValid_WithGroupsAndTeamsButMissingMatches_ReturnsFalse()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();

        foreach (var group in fixture.Groups)
        {
            for (int i = 1; i <= 4; i++)
            {
                var team = new Team($"Team_{group.GroupLetter}_{i}", new Confederation("UEFA"), 1000);
                group.AddTeamToGroup(team);
            }
        }

        var result = fixture.IsValid();

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToRoundOf32_WhenGroupMatchesAreNotPlayed_ReturnsFalse()
    {
        var fixture = CreateValidFixture();

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);

        var result = fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void AdvanceToPhase_ToRoundOf32_WhenGroupMatchesAreNotPlayed_ShouldThrowException()
    {
        var fixture = CreateValidFixture();

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.AdvanceToPhase(TournamentPhase.RoundOf32));
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToFinished_WhenCurrentPhaseIsNotFinal_ReturnsFalse()
    {
        var fixture = CreateValidFixture();
    
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
    
        var result = fixture.CanAdvanceToPhase(TournamentPhase.Finished);
    
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void AdvanceToPhase_WhenInvalid_ShouldKeepCurrentPhase()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);

        Assert.ThrowsException<InvalidOperationException>(() =>
            fixture.AdvanceToPhase(TournamentPhase.GroupStage));

        Assert.AreEqual(TournamentPhase.NotStarted, fixture.CurrentPhase);
    }
    
    [TestMethod]
    public void IsStillValid_WhenFixtureHasNoGroups_ReturnsFalse()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);

        var teams = new List<Team>();
        var stadiums = new List<Stadium>();

        var result = fixture.IsStillValid(teams, stadiums);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void CanAdvanceToPhase_ToRoundOf16_WhenCurrentPhaseIsNotRoundOf32_ReturnsFalse()
    {
        var fixture = CreateValidFixture();

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);

        var result = fixture.CanAdvanceToPhase(TournamentPhase.RoundOf16);

        Assert.IsFalse(result);
    }
    
    private Fixture CreateValidFixture()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
    
        foreach (var group in fixture.Groups)
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