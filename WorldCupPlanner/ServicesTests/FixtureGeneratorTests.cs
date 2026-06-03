using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using ServicesTests.Stubs;
using IRepository;

namespace ServicesTests;

[TestClass]
public class FixtureGeneratorTests
{
    private WorldCupPlannerContext _context = null!;
    private TeamRepository _teamRepository = null!;
    private IStadiumRepository _stadiumRepository = null!;
    private FixtureRepository _fixtureRepository = null!;
    private FixtureGenerator _generator = null!;
    private TournamentConfig _config = null!;
    private IDateTimeMatch _clock = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);

        _teamRepository = new TeamRepository(_context);
        _fixtureRepository = new FixtureRepository(_context);
        _stadiumRepository = new StadiumRepositoryStub();
        _config = new TournamentConfig();
        _clock = new ClockStub(new DateTime(2026, 1, 1));
        _generator = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, _clock);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_context != null)
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    private void AddTeams(int count)
    {
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7),
            ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };

        int teamIndex = 1;
        int rankingBase = 500;
        foreach (var (name, limit) in confederations)
        {
            int teamsToAdd = Math.Min(limit, count - teamIndex + 1);
            for (int i = 1; i <= teamsToAdd && teamIndex <= count; i++)
            {
                _teamRepository.Add(new Team($"Team_{teamIndex}",
                    new Confederation(name), rankingBase + teamIndex));
                teamIndex++;
            }
        }
    }

    private void AddStadiums(int count)
    {
        for (int i = 1; i <= count; i++)
            _stadiumRepository.AddStadium(new Stadium($"Estadio_{i}", $"Ciudad_{i}", "", 50000));
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithLessThan48Teams_ThrowsException()
    {
        AddTeams(40);
        AddStadiums(4);
        try
        {
            _generator.GenerateGroupsStageFixture();
            Assert.Fail("Debería lanzar una excepción");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("48 equipos"));
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithLessThan4Stadiums_ThrowsException()
    {
        AddTeams(48);
        AddStadiums(2);
        try
        {
            _generator.GenerateGroupsStageFixture();
            Assert.Fail("Debería lanzar una excepción");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("4 estadios"));
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithValidData_ReturnsFixture()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        Assert.IsNotNull(fixture);
        Assert.AreEqual(TournamentPhase.GroupStage, fixture.CurrentPhase);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_Creates12Groups()
    {
        AddTeams(48);
        AddStadiums(4);
        Assert.AreEqual(12, _generator.GenerateGroupsStageFixture().Groups.Count);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_DistributesTeamsToGroups()
    {
        AddTeams(48);
        AddStadiums(4);
        var total = _generator.GenerateGroupsStageFixture().Groups.Sum(g => g.TeamsInGroup.Count);
        Assert.AreEqual(48, total);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_EachGroupHas4Teams()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
            Assert.AreEqual(4, group.TeamsInGroup.Count);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_EachGroupHas6Matches()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
            Assert.AreEqual(6, group.MatchesInGroup.Count);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchesHaveCorrectDates()
    {
        var startDate = new DateTime(2026, 6, 1);
        var config    = new TournamentConfig(123, 123, 123, 123, startDate, 3, 3);
        var generator = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, config,
            new ClockStub(new DateTime(2026, 1, 1)));

        AddTeams(48);
        AddStadiums(4);

        var partidos = generator.GenerateGroupsStageFixture().Groups[0].MatchesInGroup.ToList();

        Assert.AreEqual(startDate.Date,              partidos[0].DateMatch.Date);
        Assert.AreEqual(startDate.Date,              partidos[1].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(3).Date,   partidos[2].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(3).Date,   partidos[3].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(6).Date,   partidos[4].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(6).Date,   partidos[5].DateMatch.Date);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchesHaveDifferentTeams()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
            foreach (var match in group.MatchesInGroup)
                Assert.AreNotEqual(match.HomeTeam.Name, match.AwayTeam.Name);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_NoGroupHasMoreThan2UefaTeams()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
        {
            int uefaCount = group.TeamsInGroup.Count(t => t.Confederation.Name == "UEFA");
            Assert.IsTrue(uefaCount <= 2, $"Grupo {group.GroupLetter} tiene {uefaCount} equipos UEFA (máximo 2)");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_NonUefaConfederationsNotRepeatedInGroup()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
        {
            var nonUefa = group.TeamsInGroup
                .Where(t => t.Confederation.Name != "UEFA")
                .Select(t => t.Confederation.Name)
                .ToList();
            Assert.AreEqual(nonUefa.Count, nonUefa.Distinct().Count(), $"Grupo {group.GroupLetter} tiene confederaciones no-UEFA repetidas");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_IsDeterministicWithSameSeed()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture1 = _generator.GenerateGroupsStageFixture();

        var options2 = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context2 = new WorldCupPlannerContext(options2);
        _teamRepository    = new TeamRepository(context2);
        _stadiumRepository = new StadiumRepositoryStub();
        _fixtureRepository = new FixtureRepository(context2);

        AddTeams(48);
        AddStadiums(4);

        var fixture2 = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, _clock)
            .GenerateGroupsStageFixture();

        for (int i = 0; i < fixture1.Groups.Count; i++)
        {
            var teams1 = fixture1.Groups[i].TeamsInGroup.Select(t => t.Name).ToList();
            var teams2 = fixture2.Groups[i].TeamsInGroup.Select(t => t.Name).ToList();
            CollectionAssert.AreEqual(teams1, teams2, $"Grupo {fixture1.Groups[i].GroupLetter} difiere entre generaciones con la misma semilla");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithTiedRankings_UsesFixtureSeedForTieBreak()
    {
        AddTeamsWithSameRanking();
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        Assert.AreEqual(12, fixture.Groups.Count);
        Assert.AreEqual(48, fixture.Groups.Sum(g => g.TeamsInGroup.Count));
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithTiedRankings_IsDeterministicWithSameSeed()
    {
        AddTeamsWithSameRanking();
        AddStadiums(4);
        var fixture1 = _generator.GenerateGroupsStageFixture();

        var options2 = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context2 = new WorldCupPlannerContext(options2);
        _teamRepository    = new TeamRepository(context2);
        _stadiumRepository = new StadiumRepositoryStub();
        _fixtureRepository = new FixtureRepository(context2);

        AddTeamsWithSameRanking();
        AddStadiums(4);

        var fixture2 = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, _clock)
            .GenerateGroupsStageFixture();


        for (int i = 0; i < fixture1.Groups.Count; i++)
        {
            var teams1 = fixture1.Groups[i].TeamsInGroup.Select(t => t.Name).ToList();
            var teams2 = fixture2.Groups[i].TeamsInGroup.Select(t => t.Name).ToList();
            CollectionAssert.AreEqual(teams1, teams2, $"Grupo {fixture1.Groups[i].GroupLetter} difiere con misma semilla y rankings empatados");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithTiedRankings_DifferentSeedProducesDifferentOrder()
    {
        AddTeamsWithSameRanking();
        AddStadiums(4);

        var config2    = new TournamentConfig(99999, 67890, 13579, 24680, new DateTime(2026, 6, 1), 3, 3);
        var generator2 = new FixtureGenerator(
            _teamRepository, _stadiumRepository, _fixtureRepository, config2, _clock);

        var fixture1 = _generator.GenerateGroupsStageFixture();

        _fixtureRepository = new FixtureRepository(_context);
        var fixture2       = generator2.GenerateGroupsStageFixture();

        bool isDifferent = false;
        for (int i = 0; i < fixture1.Groups.Count; i++)
        {
            var teams1 = fixture1.Groups[i].TeamsInGroup.Select(t => t.Name).ToList();
            var teams2 = fixture2.Groups[i].TeamsInGroup.Select(t => t.Name).ToList();
            if (!teams1.SequenceEqual(teams2)) { isDifferent = true; break; }
        }
        Assert.IsTrue(isDifferent, "Semillas distintas deberían producir distribuciones distintas");
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchDatesLastRoundAreSameDayAndHour()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
        {
            var partidos = group.MatchesInGroup.ToList();
            Assert.AreEqual(partidos[4].DateMatch, partidos[5].DateMatch, $"Grupo {group.GroupLetter}: últimos 2 partidos deben ser el mismo día y hora");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchDatesRound2IsAfterRound1()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
        {
            var p = group.MatchesInGroup.ToList();
            Assert.IsTrue(p[2].DateMatch > p[0].DateMatch, $"Grupo {group.GroupLetter}: jornada 2 debe ser después de jornada 1");
            Assert.IsTrue(p[4].DateMatch > p[2].DateMatch, $"Grupo {group.GroupLetter}: jornada 3 debe ser después de jornada 2");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_AllMatchesHaveStadium()
    {
        AddTeams(48);
        AddStadiums(4);
        foreach (var group in _generator.GenerateGroupsStageFixture().Groups)
            foreach (var match in group.MatchesInGroup)
                Assert.IsNotNull(match.Stadium);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_GroupLettersAreAtoL()
    {
        AddTeams(48);
        AddStadiums(4);
        var letras   = _generator.GenerateGroupsStageFixture().Groups.Select(g => g.GroupLetter).OrderBy(c => c).ToList();
        var esperadas = Enumerable.Range('A', 12).Select(c => (char)c).ToList();
        CollectionAssert.AreEqual(esperadas, letras);
    }

    private void AddTeamsWithSameRanking()
    {
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7),
            ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };
        int teamIndex = 1;
        foreach (var (name, limit) in confederations)
            for (int i = 0; i < limit; i++)
                _teamRepository.Add(new Team($"Team_{teamIndex++}", new Confederation(name), 1000));
    }
}
