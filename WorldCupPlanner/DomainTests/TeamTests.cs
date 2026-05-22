using Domain.Entities;

namespace DomainTests;

[TestClass]
public class TeamTests
{
    private Confederation _defaultConfederation = null!;
    private const int MinFifaRanking = 300;
    private const int MaxFifaRanking = 2500;
    private const int MaxNameLength = 60;

    [TestInitialize]
    public void SetUp()
    {
        _defaultConfederation = new Confederation("AFC");
    }

    [TestMethod]
    public void CreateTeam_WithValidData_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, MinFifaRanking + 1);
        VerifyTeam(team, "Uruguay", MinFifaRanking + 1);
    }

    [TestMethod]
    public void CreateTeam_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team("", _defaultConfederation, MinFifaRanking + 1));
    }

    [TestMethod]
    public void CreateTeam_WithWhiteSpaceName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team("   ", _defaultConfederation, MinFifaRanking));
    }

    [TestMethod]
    public void CreateTeam_WithNameTooLong_ShouldThrowException()
    {
        string nameWith61Characters = new string('A', MaxNameLength + 1);

        Assert.ThrowsException<ArgumentException>(() =>
            new Team(nameWith61Characters, _defaultConfederation, MinFifaRanking + 1));
    }

    [TestMethod]
    public void CreateTeam_WithMinimumNameLength_ShouldSucceed()
    {
        var team = new Team("A", _defaultConfederation, MinFifaRanking);
        VerifyTeam(team, "A", MinFifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithMaximumNameLength_ShouldSucceed()
    {
        string maxName = new string('A', MaxNameLength);

        var team = new Team(maxName, _defaultConfederation, MinFifaRanking);

        VerifyTeam(team, maxName, MinFifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithNullConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new Team("Uruguay", null!, MinFifaRanking + 1));
    }

    [TestMethod]
    public void CreateTeam_WithRankingBelowMinimum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team("Uruguay", _defaultConfederation, MinFifaRanking - 1));
    }

    [TestMethod]
    public void CreateTeam_WithRankingAboveMaximum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team("Uruguay", _defaultConfederation, MaxFifaRanking + 1));
    }

    [TestMethod]
    public void CreateTeam_WithValidIntermediateRanking_ShouldSucceed()
    {
        int intermediateRanking = (MinFifaRanking + MaxFifaRanking) / 2;

        var team = new Team("Uruguay", _defaultConfederation, intermediateRanking);

        VerifyTeam(team, "Uruguay", intermediateRanking);
    }

    [TestMethod]
    public void CreateTeam_WithMaximumRanking_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, MaxFifaRanking);
        VerifyTeam(team, "Uruguay", MaxFifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithMinimumRanking_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, MinFifaRanking);
        VerifyTeam(team, "Uruguay", MinFifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithNullName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Team(null!, _defaultConfederation, MinFifaRanking));
    }

    [TestMethod]
    public void CreateTeam_WithLowerBoundaryRanking_ShouldStoreRanking()
    {
        var team = new Team("Uruguay", _defaultConfederation, MinFifaRanking);

        Assert.AreEqual(MinFifaRanking, team.FifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithUpperBoundaryRanking_ShouldStoreRanking()
    {
        var team = new Team("Uruguay", _defaultConfederation, MaxFifaRanking);

        Assert.AreEqual(MaxFifaRanking, team.FifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithConfederationWithEmptyName_ShouldThrowArgumentException()
    {
        var confederation = new Confederation("AFC");

        var field = typeof(Confederation).GetField(
                        "_name",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?? typeof(Confederation).GetField(
                        "<Name>k__BackingField",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(field);

        field.SetValue(confederation, "");

        Assert.ThrowsException<ArgumentException>(() =>
            new Team("Uruguay", confederation, MinFifaRanking));
    }

    [TestMethod]
    public void Constructor_ShouldSetCurrentRankingEqualToFifaRanking()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        Assert.AreEqual(1800, team.CurrentRanking);
    }

    [TestMethod]
    public void UpdateCurrentRanking_WithValidRanking_ShouldUpdateCurrentRanking()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        team.UpdateCurrentRanking(2000);

        Assert.AreEqual(2000, team.CurrentRanking);
    }

    [TestMethod]
    public void UpdateCurrentRanking_ShouldNotModifyFifaRanking()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        team.UpdateCurrentRanking(2000);

        Assert.AreEqual(1800, team.FifaRanking);
    }

    [TestMethod]
    public void UpdateCurrentRanking_WithMinRanking_ShouldUpdateCurrentRanking()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        team.UpdateCurrentRanking(Team.MinRanking);

        Assert.AreEqual(Team.MinRanking, team.CurrentRanking);
    }

    [TestMethod]
    public void UpdateCurrentRanking_WithMaxRanking_ShouldUpdateCurrentRanking()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        team.UpdateCurrentRanking(Team.MaxRanking);

        Assert.AreEqual(Team.MaxRanking, team.CurrentRanking);
    }

    [TestMethod]
    public void UpdateCurrentRanking_WithRankingBelowMin_ShouldThrowException()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        Assert.ThrowsException<ArgumentException>(() =>
            team.UpdateCurrentRanking(Team.MinRanking - 1));
    }

    [TestMethod]
    public void UpdateCurrentRanking_WithRankingAboveMax_ShouldThrowException()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        Assert.ThrowsException<ArgumentException>(() =>
            team.UpdateCurrentRanking(Team.MaxRanking + 1));
    }

    [TestMethod]
    public void UpdateCurrentRanking_WithInvalidRanking_ShouldNotModifyCurrentRanking()
    {
        var team = new Team("Uruguay", new Confederation("CONMEBOL"), 1800);

        try
        {
            team.UpdateCurrentRanking(9999);
        }
        catch
        {
        }

        Assert.AreEqual(1800, team.CurrentRanking);
    }

    [TestMethod]
    public void TeamWithValidFlagBase64_CreatingTeam_StoresFlagBase64()
    {
        string flagBase64 = "data:image/png;base64,abc123";

        var team = new Team("Uruguay", _defaultConfederation, 1500, flagBase64);

        Assert.AreEqual(flagBase64, team.FlagBase64);
    }

    [TestMethod]
    public void TeamWithoutFlagBase64_CreatingTeam_StoresEmptyFlag()
    {
        var team = new Team("Uruguay", _defaultConfederation, 1500);

        Assert.AreEqual(string.Empty, team.FlagBase64);
    }

    private void VerifyTeam(Team team, string expectedName, int expectedRanking)
    {
        Assert.IsNotNull(team);
        Assert.AreEqual(expectedName, team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(expectedRanking, team.FifaRanking);
    }
}