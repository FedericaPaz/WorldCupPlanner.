using Domain;
using DomainTests.Stubs;
using Repository;

namespace RepositoryTests;

[TestClass]
public class MatchRepositoryTests
{
    [TestInitialize]
    public void Setup()
    {
        Match.ResetIdCounter();
    }

    [TestMethod]
    public void AddMatch_WithValidMatch_ShouldStoreMatch()
    {
        var repository = new MatchRepository();
        var clock = new DateTimeMatchStub(new DateTime(2026, 6, 1));
        var result = new Match(clock.Now, new StadiumStub("Centenario"), new TeamStub("Uruguay"), new TeamStub("Argentina"), 'A', clock);

        repository.AddMatch(result);
        Assert.AreEqual(1, repository.GetAllMatches().Count);
        Assert.AreEqual(result, repository.GetAllMatches()[0]);
    }
}