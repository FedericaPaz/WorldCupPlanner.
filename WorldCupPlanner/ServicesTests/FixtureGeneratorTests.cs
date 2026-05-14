using Repository;
using Services;
using Domain;

namespace ServicesTests;

[TestClass]
public class FixtureGeneratorTests
{
    private TeamRepository _teamRepository;
    private StadiumRepository _stadiumRepository;
    private FixtureRepository _fixtureRepository;
    private FixtureGenerator _generator;
    private TournamentConfig _config;
    private IDateTimeMatch _clock;

    [TestInitialize]
    public void Setup()
    {
        _teamRepository = new TeamRepository();
        _stadiumRepository = new StadiumRepository();
        _fixtureRepository = new FixtureRepository();
        _config = new TournamentConfig();
        _clock = new ClockStub(new DateTime(2026, 1, 1));
        _generator = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, _clock);
        Match.ResetIdCounter();
    }

    private void AddTeams(int count)
    {
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7), ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };

        int teamIndex = 1;
        int rankingBase = 500;
        foreach (var (name, limit) in confederations)
        {
            int teamsToAdd = Math.Min(limit, count - teamIndex + 1);
            for (int i = 1; i <= teamsToAdd && teamIndex <= count; i++)
            {
                var team = new Team($"Team_{teamIndex}", new Confederation(name), rankingBase + teamIndex);
                _teamRepository.Add(team);
                teamIndex++;
            }
        }
    }

    private void AddStadiums(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            var stadium = new Stadium($"Estadio_{i}", $"Ciudad_{i}", "", 50000);
            _stadiumRepository.AddStadium(stadium);
        }
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
        var fixture = _generator.GenerateGroupsStageFixture();
        Assert.AreEqual(12, fixture.GetGroups.Count);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_DistributesTeamsToGroups()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        int totalEquipos = fixture.GetGroups.Sum(g => g.TeamsInGroup.Count);
        Assert.AreEqual(48, totalEquipos);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_EachGroupHas4Teams()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        foreach (var group in fixture.GetGroups)
            Assert.AreEqual(4, group.TeamsInGroup.Count);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_EachGroupHas6Matches()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        foreach (var group in fixture.GetGroups)
            Assert.AreEqual(6, group.MatchesInGroup.Count);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchesHaveCorrectDates()
    {
        var startDate = new DateTime(2026, 6, 1);
        var config = new TournamentConfig(123, 123, 123, 123, startDate, 3, 3);
        var clock = new ClockStub(new DateTime(2026, 1, 1));
        var generator = new FixtureGenerator(
            _teamRepository, _stadiumRepository, _fixtureRepository, config, clock);

        AddTeams(48);
        AddStadiums(4);

        var fixture = generator.GenerateGroupsStageFixture();
        var primerGrupo = fixture.GetGroups[0];
        var partidos = primerGrupo.MatchesInGroup.ToList();

        Assert.AreEqual(startDate.Date, partidos[0].DateMatch.Date);
        Assert.AreEqual(startDate.Date, partidos[1].DateMatch.Date);

        Assert.AreEqual(startDate.AddDays(3).Date, partidos[2].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(3).Date, partidos[3].DateMatch.Date);

        Assert.AreEqual(startDate.AddDays(6).Date, partidos[4].DateMatch.Date);
        Assert.AreEqual(startDate.AddDays(6).Date, partidos[5].DateMatch.Date);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchesHaveDifferentTeams()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        foreach (var group in fixture.GetGroups)
        {
            foreach (var match in group.MatchesInGroup)
            {
                Assert.AreNotEqual(match.HomeTeam.Name, match.AwayTeam.Name,
                    "Local y visitante no pueden ser el mismo equipo");
            }
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_NoGroupHasMoreThan2UefaTeams()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        foreach (var group in fixture.GetGroups)
        {
            int uefaCount = group.TeamsInGroup.Count(t => t.Confederation.Name == "UEFA");
            Assert.IsTrue(uefaCount <= 2,
                $"Grupo {group.GroupLetter} tiene {uefaCount} equipos UEFA (máximo 2)");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_NonUefaConfederationsNotRepeatedInGroup()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        foreach (var group in fixture.GetGroups)
        {
            var nonUefa = group.TeamsInGroup
                .Where(t => t.Confederation.Name != "UEFA")
                .Select(t => t.Confederation.Name)
                .ToList();

            var distinct = nonUefa.Distinct().ToList();
            Assert.AreEqual(nonUefa.Count, distinct.Count,
                $"Grupo {group.GroupLetter} tiene confederaciones no-UEFA repetidas");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_IsDeterministicWithSameSeed()
    {
        AddTeams(48);
        AddStadiums(4);

        var fixture1 = _generator.GenerateGroupsStageFixture();

        _teamRepository = new TeamRepository();
        _stadiumRepository = new StadiumRepository();
        _fixtureRepository = new FixtureRepository();
        AddTeams(48);
        AddStadiums(4);
        Match.ResetIdCounter();

        var generator2 = new FixtureGenerator(_teamRepository, _stadiumRepository, _fixtureRepository, _config, _clock);
        var fixture2 = generator2.GenerateGroupsStageFixture();

        for (int i = 0; i < fixture1.GetGroups.Count; i++)
        {
            var teams1 = fixture1.GetGroups[i].TeamsInGroup.Select(t => t.Name).ToList();
            var teams2 = fixture2.GetGroups[i].TeamsInGroup.Select(t => t.Name).ToList();
            CollectionAssert.AreEqual(teams1, teams2,
                $"Grupo {fixture1.GetGroups[i].GroupLetter} difiere entre generaciones con la misma semilla");
        }
    }
    
    [TestMethod]
    public void GenerateGroupsStageFixture_WithTiedRankings_UsesFixtureSeedForTieBreak()
    {
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7), ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };

        int teamIndex = 1;
        foreach (var (name, limit) in confederations)
        {
            for (int i = 0; i < limit; i++)
            {
                var team = new Team($"Team_{teamIndex}", new Confederation(name), 1000); // mismo ranking
                _teamRepository.Add(team);
                teamIndex++;
            }
        }
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        Assert.AreEqual(12, fixture.GetGroups.Count);
        Assert.AreEqual(48, fixture.GetGroups.Sum(g => g.TeamsInGroup.Count));
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithTiedRankings_IsDeterministicWithSameSeed()
    {
        int teamIndex = 1;
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7), ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };
        foreach (var (name, limit) in confederations)
            for (int i = 0; i < limit; i++)
            {
                _teamRepository.Add(new Team($"Team_{teamIndex++}", new Confederation(name), 1000));
            }
        AddStadiums(4);

        var fixture1 = _generator.GenerateGroupsStageFixture();

        _teamRepository = new TeamRepository();
        _stadiumRepository = new StadiumRepository();
        _fixtureRepository = new FixtureRepository();
        Match.ResetIdCounter();
        teamIndex = 1;
        foreach (var (name, limit) in confederations)
            for (int i = 0; i < limit; i++)
                _teamRepository.Add(new Team($"Team_{teamIndex++}", new Confederation(name), 1000));
        AddStadiums(4);

        var generator2 = new FixtureGenerator(
            _teamRepository, _stadiumRepository, _fixtureRepository, _config, _clock);
        var fixture2 = generator2.GenerateGroupsStageFixture();

        for (int i = 0; i < fixture1.GetGroups.Count; i++)
        {
            var teams1 = fixture1.GetGroups[i].TeamsInGroup.Select(t => t.Name).ToList();
            var teams2 = fixture2.GetGroups[i].TeamsInGroup.Select(t => t.Name).ToList();
            CollectionAssert.AreEqual(teams1, teams2,
                $"Grupo {fixture1.GetGroups[i].GroupLetter} difiere con misma semilla y rankings empatados");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_WithTiedRankings_DifferentSeedProducesDifferentOrder()
    {
        var confederations = new List<(string name, int limit)>
        {
            ("UEFA", 16), ("CONMEBOL", 7), ("CONCACAF", 7), ("CAF", 9), ("AFC", 8), ("OFC", 1)
        };

        int teamIndex = 1;
        foreach (var (name, limit) in confederations)
            for (int i = 0; i < limit; i++)
                _teamRepository.Add(new Team($"Team_{teamIndex++}", new Confederation(name), 1000));
        AddStadiums(4);

        var config2 = new TournamentConfig(99999, 67890, 13579, 24680,
            new DateTime(2026, 6, 1), 3, 3);
        var generator2 = new FixtureGenerator(
            _teamRepository, _stadiumRepository, _fixtureRepository, config2, _clock);

        var fixture1 = _generator.GenerateGroupsStageFixture();
        
        _fixtureRepository = new FixtureRepository();
        Match.ResetIdCounter();
        var fixture2 = generator2.GenerateGroupsStageFixture();

        bool isDifferent = false;
        for (int i = 0; i < fixture1.GetGroups.Count; i++)
        {
            var teams1 = fixture1.GetGroups[i].TeamsInGroup.Select(t => t.Name).ToList();
            var teams2 = fixture2.GetGroups[i].TeamsInGroup.Select(t => t.Name).ToList();
            if (!teams1.SequenceEqual(teams2)) { isDifferent = true; break; }
        }
        Assert.IsTrue(isDifferent, "Semillas distintas deberían producir distribuciones distintas");
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchDatesLastRoundAreSameDayAndHour()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();

        foreach (var group in fixture.GetGroups)
        {
            var partidos = group.MatchesInGroup.ToList();
            Assert.AreEqual(partidos[4].DateMatch, partidos[5].DateMatch,
                $"Grupo {group.GroupLetter}: últimos 2 partidos deben ser el mismo día y hora");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_MatchDatesRound2IsAfterRound1()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();

        foreach (var group in fixture.GetGroups)
        {
            var partidos = group.MatchesInGroup.ToList();
            Assert.IsTrue(partidos[2].DateMatch > partidos[0].DateMatch,
                $"Grupo {group.GroupLetter}: jornada 2 debe ser después de jornada 1");
            Assert.IsTrue(partidos[4].DateMatch > partidos[2].DateMatch,
                $"Grupo {group.GroupLetter}: jornada 3 debe ser después de jornada 2");
        }
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_AllMatchesHaveStadium()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        foreach (var group in fixture.GetGroups)
            foreach (var match in group.MatchesInGroup)
                Assert.IsNotNull(match.Stadium);
    }

    [TestMethod]
    public void GenerateGroupsStageFixture_GroupLettersAreAtoL()
    {
        AddTeams(48);
        AddStadiums(4);
        var fixture = _generator.GenerateGroupsStageFixture();
        var letras = fixture.GetGroups.Select(g => g.GroupLetter).OrderBy(c => c).ToList();
        var esperadas = Enumerable.Range('A', 12).Select(c => (char)c).ToList();
        CollectionAssert.AreEqual(esperadas, letras);
    }
    
}