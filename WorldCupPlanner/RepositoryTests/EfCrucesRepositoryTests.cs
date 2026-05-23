using Domain.Entities;
using Domain.Enums;
using DomainTests.Stubs;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryTests;

[TestClass]
public class EfCrucesRepositoryTests
{
    [TestMethod]
    public void AddCruces_WithValidCruces_ShouldPersistCruces()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new WorldCupPlannerContext(options);
        var repository = new EfCrucesRepository(context);

        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var confederation = new Confederation("CONMEBOL");
        var homeTeam = new Team("Uruguay", confederation, 1500);
        var awayTeam = new Team("Argentina", confederation, 1500);
        var stadium = new Stadium("Centenario", "Montevideo", "Estadio principal", 60000);
        var match = new Match(1, clock.Now, stadium, homeTeam, awayTeam, 'N', clock);

        var cruce = new Cruce("A1", match, TournamentPhase.RoundOf32);

        repository.AddCruces(new List<Cruce> { cruce });

        var result = repository.GetAllCruces();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("A1", result[0].Name);
        Assert.AreEqual(TournamentPhase.RoundOf32, result[0].Phase);
    }
}