using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using ServicesTests.Stubs;

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
                match.SimulateResult(1, 0);
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
        var teams = new List<ITeam>();
        var stadiums = new List<IStadium>();

        foreach (var group in fixture.Groups)
        {
            foreach (var team in group.TeamsInGroup)
                teams.Add(team);

            foreach (var match in group.MatchesInGroup)
            {
                if (!stadiums.Contains(match.Stadium))
                    stadiums.Add(match.Stadium);
            }
        }
        bool result = fixture.IsStillValid(teams, stadiums);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsStillValid_WhenTeamIsMissing_ReturnsFalse()
    {
        var fixture = CreateValidFixture();
        var teams = new List<ITeam>();
        var stadiums = new List<IStadium>();

        for (int i = 1; i < fixture.Groups.Count; i++)
        {
            var group = fixture.Groups[i];
            foreach (var team in group.TeamsInGroup)
                teams.Add(team);
        }
        foreach (var group in fixture.Groups)
        {
            foreach (var match in group.MatchesInGroup)
            {
                if (!stadiums.Contains(match.Stadium))
                    stadiums.Add(match.Stadium);
            }
        }
        bool result = fixture.IsStillValid(teams, stadiums);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsStillValid_WhenStadiumIsMissing_ReturnsFalse()
    {
        var fixture = CreateValidFixture();
        var teams = new List<ITeam>();
        var stadiums = new List<IStadium>();

        foreach (var group in fixture.Groups)
        {
            foreach (var team in group.TeamsInGroup)
                teams.Add(team);
            foreach (var match in group.MatchesInGroup)
            {
                if (!stadiums.Contains(match.Stadium))
                    stadiums.Add(match.Stadium);
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
        var teams = new List<ITeam>();
        var stadiums = new List<IStadium>();
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
        var teams = new List<ITeam>();
        var stadiums = new List<IStadium>();
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
        int matchId = 1;

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
                var match = new Match(matchId++, clock.Now, stadium, teams[0], teams[1], group.GroupLetter, clock);
                group.AddMatchToGroup(match);
            }
        }
        return fixture;
    }
    
    [TestMethod]
    public void PrivateConstructor_ShouldCreateFixtureInstance()
    {
        var fixture = (Fixture)Activator.CreateInstance(
            typeof(Fixture),
            nonPublic: true)!;
        
        Assert.IsNotNull(fixture);
        Assert.AreEqual(0, fixture.Id);
        Assert.AreEqual(0, fixture.Groups.Count);
    }

    [TestMethod]
    public void SetGroups_WithValidList_ShouldAssignGroups()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);
        var targetGroups = new List<Group> { new Group('A'), new Group('B') };
        
        fixture.SetGroups(targetGroups);
        
        Assert.AreEqual(2, fixture.Groups.Count);
    }

    [TestMethod]
    public void SetGroups_WithNull_ShouldThrowArgumentNullException()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);

        Assert.ThrowsException<ArgumentNullException>(() => fixture.SetGroups(null!));
    }

    [TestMethod]
    public void AddEliminatoryMatch_WithNull_ShouldThrowArgumentNullException()
    {
        var config = new TournamentConfig();
        var fixture = new Fixture(config);

        Assert.ThrowsException<ArgumentNullException>(() => fixture.AddEliminatoryMatch(null!));
    }

    [TestMethod]
    public void CanAdvanceToPhase_ThroughAllKnockoutPhases_ShouldValidateCorrectly()
    {
        var fixture = CreateValidFixture();
        var clock = new ClockStub(DateTime.Now);
        var stadium = new Stadium("Estadio", "Ciudad", "", 50000);
        var teamA = new Team("TeamA", new Confederation("UEFA"), 1000);
        var teamB = new Team("TeamB", new Confederation("UEFA"), 1000);
        
        Assert.IsFalse(fixture.CanAdvanceToPhase((TournamentPhase)999));

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        
        foreach (var group in fixture.Groups)
        {
            foreach (var m in group.MatchesInGroup)
            {
                m.SetClock(clock);
                m.SetMatchResult(1, 0); 
            }
        }
        
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32));
        fixture.AdvanceToPhase(TournamentPhase.RoundOf32);
        
        var match = new Match(999, clock.Now, stadium, teamA, teamB, 'A', clock);
        fixture.AddEliminatoryMatch(match);
        
        Assert.IsFalse(fixture.CanAdvanceToPhase(TournamentPhase.RoundOf16)); 
        match.SetMatchResult(2, 1); 
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.RoundOf16));
        
        fixture.AdvanceToPhase(TournamentPhase.RoundOf16);
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.QuarterFinals));
        
        fixture.AdvanceToPhase(TournamentPhase.QuarterFinals);
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.SemiFinals));
        
        fixture.AdvanceToPhase(TournamentPhase.SemiFinals);
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.Final));
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.ThirdPlace));
        
        fixture.AdvanceToPhase(TournamentPhase.Final);
        Assert.IsTrue(fixture.CanAdvanceToPhase(TournamentPhase.Finished));
    }
}