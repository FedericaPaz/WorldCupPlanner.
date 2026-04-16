using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DomainTests;

[TestClass]
public class TeamTests
{
    private Confederation _defaultConfederation;
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
        var team = new Team("Uruguay", _defaultConfederation, 301);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(301, team.FifaRanking);
    }
    
    [TestMethod]
    public void CreateTeam_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", _defaultConfederation, 301));
    }
    
    [TestMethod]
    public void CreateTeam_WithWhiteSpaceName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("   ", _defaultConfederation, MinFifaRanking));
    }

    [TestMethod]
    public void CreateTeam_WithNameTooLong_ShouldThrowException()
    {
        string nameWith61Characters = new string('A', MaxNameLength + 1); 
        Assert.ThrowsException<ArgumentException>(() => new Team(nameWith61Characters, _defaultConfederation, 301));
    }

    [TestMethod]
    public void CreateTeam_WithNullConfederation_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", null, 301));
    }

    [TestMethod]
    public void CreateTeam_WithRankingBelowMinimum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _defaultConfederation, 299));
        
    }

    [TestMethod]
    public void CreateTeam_WithRankingAboveMaximum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _defaultConfederation, 2501));
    }

    [TestMethod]
    public void CreateTeam_WithRankingAboveMinimum_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, 301);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(301, team.FifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithRankingBelowMaximum_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, 2499);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(2499, team.FifaRanking);
    }

    [TestMethod]
    public void CreateTeam_WithMaximumRanking_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, MaxFifaRanking);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(MaxFifaRanking, team.FifaRanking);
    }
    
    [TestMethod]
    public void CreateTeam_WithMinimumRanking_ShouldSucceed()
    {
        var team = new Team("Uruguay", _defaultConfederation, MinFifaRanking);
        Assert.IsNotNull(team);
        Assert.AreEqual("Uruguay", team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(MinFifaRanking, team.FifaRanking);
    }
}