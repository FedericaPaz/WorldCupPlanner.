using Repository;
using Domain.Entities;
using Domain.Enums;
using ServicesTests.Stubs;

namespace RepositoryTests;

[TestClass]
public class CrucesRepositoryTests
{
    private static readonly DateTime FechaCreacionClock  = new DateTime(2027, 6, 10);
    private static readonly DateTime FechaPartido        = new DateTime(2027, 6, 12);
    private static readonly DateTime FechaResultadoClock = new DateTime(2027, 6, 15);
    
    private CrucesRepository _crucesRepository;

    [TestInitialize]
    public void Setup()
    {
        _crucesRepository = new CrucesRepository();
    }
    
    private List<Cruce> CreateCruces(int cant, TournamentPhase phase)
    {
        List<Cruce> cruces = new List<Cruce>();
        for (int i = 0; i < cant; i++)
        {
            Team local     = new Team($"Local{i}",     new Confederation("UEFA"),    500);
            Team visitante = new Team($"Visitante{i}", new Confederation("CONMEBOL"), 501);
            Match partido = new Match(FechaPartido,
                new Stadium("Estadio", "Ciudad", "Desc", 50000),
                local, visitante, 'A',
                new ClockStub(FechaCreacionClock));
            partido.SetClock(new ClockStub(FechaResultadoClock));
            partido.SetMatchResult(2, 0);
            cruces.Add(new Cruce($"C{i}", partido, phase));
        }
        return cruces;
    }
    
    [TestMethod]
    public void AddCruces_WhenCrucesAreAdded_StoresThem()
    {
        List<Cruce> cruces = CreateCruces(3, TournamentPhase.RoundOf32);
        _crucesRepository.AddCruces(cruces);
        List<Cruce> result = _crucesRepository.GetAllCruces();
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void GetAllCruces_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        List<Cruce> result = _crucesRepository.GetAllCruces();
        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void GetAllCruces_WhenCrucesExist_ReturnsAllCruces()
    {
        List<Cruce> cruces = CreateCruces(5, TournamentPhase.RoundOf16);
        _crucesRepository.AddCruces(cruces);
        List<Cruce> result = _crucesRepository.GetAllCruces();
        Assert.AreEqual(5, result.Count);
    }
    
    [TestMethod]
    public void HasCruces_WhenRepositoryIsEmpty_ReturnsFalse()
    {
        bool result = _crucesRepository.HasCruces();
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void HasCruces_WhenRepositoryHasCruces_ReturnsTrue()
    {
        List<Cruce> cruces = CreateCruces(2, TournamentPhase.RoundOf32);
        _crucesRepository.AddCruces(cruces);
        bool result = _crucesRepository.HasCruces();
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void RemoveCruces_WhenRepositoryHasCruces_RemovesAll()
    {
        List<Cruce> cruces = CreateCruces(4, TournamentPhase.QuarterFinals);
        _crucesRepository.AddCruces(cruces);
        _crucesRepository.RemoveCruces();
        List<Cruce> result = _crucesRepository.GetAllCruces();
        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void RemoveCruces_WhenRepositoryHasCruces_HasCrucesReturnsFalse()
    {
        List<Cruce> cruces = CreateCruces(4, TournamentPhase.SemiFinals);
        _crucesRepository.AddCruces(cruces);
        _crucesRepository.RemoveCruces();
        bool result = _crucesRepository.HasCruces();
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void AddCruces_WhenCalledTwice_AddsAllCruces()
    {
        List<Cruce> cruces1 = CreateCruces(2, TournamentPhase.RoundOf32);
        List<Cruce> cruces2 = CreateCruces(3, TournamentPhase.RoundOf16);
        _crucesRepository.AddCruces(cruces1);
        _crucesRepository.AddCruces(cruces2);
        List<Cruce> result = _crucesRepository.GetAllCruces();
        Assert.AreEqual(5, result.Count);
    }
}