using Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ServicesTests.Stubs;

namespace RepositoryTests;

[TestClass]
public class CrucesRepositoryTests
{
    private WorldCupPlannerContext _context = null!;
    private CrucesRepository _crucesRepository = null!;
    private static readonly DateTime FechaCreacionClock  = new DateTime(2027, 6, 10);
    private static readonly DateTime FechaPartido        = new DateTime(2027, 6, 12);
    private static readonly DateTime FechaResultadoClock = new DateTime(2027, 6, 15);
    private int _nextId;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new  WorldCupPlannerContext(options);
        
        _crucesRepository = new CrucesRepository(_context);
        
        _nextId = 1;
    }

    
    private List<Cruce> CreateCruces(int cant, TournamentPhase phase)
    {
        var cruces = new List<Cruce>();

        for (int i = 0; i < cant; i++)
        {
            int uniqueId = _nextId++; 

            var local = new Team($"Local{uniqueId}", new Confederation("UEFA"), 500);
            var visitante = new Team($"Visitante{uniqueId}", new Confederation("CONMEBOL"), 501);

            var estadio = new Stadium(
                $"Estadio{uniqueId}",
                "Ciudad",
                "Desc",
                50000);

            var partido = new Match(
                uniqueId, 
                FechaPartido,
                estadio,
                local,
                visitante,
                'A',
                new ClockStub(FechaCreacionClock));

            partido.SetClock(new ClockStub(FechaResultadoClock));
            partido.SetMatchResult(2, 0);
            
            _context.Teams.Add(local);
            _context.Teams.Add(visitante);
            _context.Stadiums.Add(estadio);
            _context.Matches.Add(partido);

            cruces.Add(new Cruce($"Cruce_{phase}_{uniqueId}", partido, phase));
        }

        return cruces;
    }

    [TestMethod]
    public void AddCruces_WhenCrucesAreAdded_StoresThem()
    {
        _crucesRepository.AddCruces(CreateCruces(3, TournamentPhase.RoundOf32));
    }

    [TestMethod]
    public void GetAllCruces_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        Assert.AreEqual(0, _crucesRepository.GetAllCruces().Count);
    }

    [TestMethod]
    public void GetAllCruces_WhenCrucesExist_ReturnsAllCruces()
    {
        _crucesRepository.AddCruces(CreateCruces(5, TournamentPhase.RoundOf16));
        Assert.AreEqual(5, _crucesRepository.GetAllCruces().Count);
    }

    [TestMethod]
    public void HasCruces_WhenRepositoryIsEmpty_ReturnsFalse()
    {
        Assert.IsFalse(_crucesRepository.HasCruces());
    }

    [TestMethod]
    public void HasCruces_WhenRepositoryHasCruces_ReturnsTrue()
    {
        _crucesRepository.AddCruces(CreateCruces(2, TournamentPhase.RoundOf32));
        Assert.IsTrue(_crucesRepository.HasCruces());
    }

    [TestMethod]
    public void RemoveCruces_WhenRepositoryHasCruces_RemovesAll()
    {
        _crucesRepository.AddCruces(CreateCruces(4, TournamentPhase.QuarterFinals));
        _crucesRepository.RemoveCruces();
        Assert.AreEqual(0, _crucesRepository.GetAllCruces().Count);
    }

    [TestMethod]
    public void RemoveCruces_WhenRepositoryHasCruces_HasCrucesReturnsFalse()
    {
        _crucesRepository.AddCruces(CreateCruces(4, TournamentPhase.SemiFinals));
        _crucesRepository.RemoveCruces();
        Assert.IsFalse(_crucesRepository.HasCruces());
    }

    [TestMethod]
    public void AddCruces_WhenCalledTwice_AddsAllCruces()
    {
        _crucesRepository.AddCruces(CreateCruces(2, TournamentPhase.RoundOf32));
        _crucesRepository.AddCruces(CreateCruces(3, TournamentPhase.RoundOf16));
        Assert.AreEqual(5, _crucesRepository.GetAllCruces().Count);
    }
    
    [TestMethod]
    public void AddCruces_WithNullCruces_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => _crucesRepository.AddCruces(null!));
    }
}