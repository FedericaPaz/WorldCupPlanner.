// TODO: Implementar tests de unicidad de nombre en la capa de Servicio (TeamService).
// Esta validación requiere acceso al repositorio/colección completa, por lo que corresponde a un Servicio de Dominio y no a la validación intrínseca de la entidad.

using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        var team = new Team("Uruguay", _defaultConfederation, MinFifaRanking+1);
        VerifyTeam(team, "Uruguay", MinFifaRanking+1);
    }
    
    [TestMethod]
    public void CreateTeam_WithEmptyName_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("", _defaultConfederation, MinFifaRanking+1));
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
        Assert.ThrowsException<ArgumentException>(() => new Team(nameWith61Characters, _defaultConfederation, MinFifaRanking+1));
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
        Assert.ThrowsException<ArgumentNullException>(() => new Team("Uruguay", null!, MinFifaRanking+1));
    }

    [TestMethod]
    public void CreateTeam_WithRankingBelowMinimum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _defaultConfederation, MinFifaRanking-1));
    }

    [TestMethod]
    public void CreateTeam_WithRankingAboveMaximum_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Team("Uruguay", _defaultConfederation, MaxFifaRanking+1));
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
    
    
    private void VerifyTeam(Team team, string expectedName, int expectedRanking)
    {
        Assert.IsNotNull(team);
        Assert.AreEqual(expectedName, team.Name);
        Assert.AreEqual(_defaultConfederation, team.Confederation);
        Assert.AreEqual(expectedRanking, team.FifaRanking);
    }
}