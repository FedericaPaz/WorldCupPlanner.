using Domain.Entities;
using Domain.Enums;
using DomainTests.Stubs;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class ExportServiceTests
{
    private UserQueryServiceStub _userQueryServiceStub = null!;
    private LogRepositoryStub _logRepository = null!;
    private LogService _logService = null!;
    private ExportService _service = null!;
    private FixtureRepositoryStub _fixtureRepository = null!;
    private MatchRepositoryStub _matchRepository = null!;
    private CrucesRepositoryStub _crucesRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _userQueryServiceStub = new UserQueryServiceStub();
        _logRepository = new LogRepositoryStub();
        _logService = new LogService(_logRepository);

        _fixtureRepository = new FixtureRepositoryStub();
        _matchRepository = new MatchRepositoryStub();
        _crucesRepository = new CrucesRepositoryStub();

        var fixture = new Fixture(new TournamentConfig());
        fixture.InitializeGroups();
        _fixtureRepository.AddFixture(fixture);

        _service = new ExportService(
            _userQueryServiceStub,
            _logService,
            _fixtureRepository,
            _matchRepository,
            _crucesRepository);
    }

    [TestCleanup]
    public void Cleanup()
    {
        DeleteGeneratedExportFiles();
    }

    private void AddUser(string email, UserRole role)
    {
        _userQueryServiceStub.AddUser(email, role);
    }

    private static Team CreateTeam(string name, string confederation = "CONMEBOL", int ranking = 1500)
    {
        return new Team(name, new Confederation(confederation), ranking);
    }

    private static Stadium CreateStadium(string name)
    {
        return new Stadium(name, "Montevideo", null, 60000);
    }

    private Match CreateMatch(
        int id,
        string homeName = "Uruguay",
        string awayName = "Argentina",
        char groupLetter = 'A',
        int? homeGoals = null,
        int? awayGoals = null)
    {
        var home = CreateTeam(homeName);
        var away = CreateTeam(awayName, "UEFA");
        var stadium = CreateStadium("Centenario");
        var clock = new DateTimeMatchStub(new DateTime(2027, 1, 1));

        var match = new Match(
            id,
            new DateTime(2027, 6, 10, 14, 0, 0),
            stadium,
            home,
            away,
            groupLetter,
            clock);

        if (homeGoals.HasValue && awayGoals.HasValue)
            match.SimulateResult(homeGoals.Value, awayGoals.Value);

        _matchRepository.AddMatch(match);

        var fixture = _fixtureRepository.GetFixture();
        var group = fixture.Groups.First(g => g.GroupLetter == groupLetter);

        if (!group.TeamsInGroup.Any(t => t.Name == home.Name))
            group.AddTeamToGroup(home);

        if (!group.TeamsInGroup.Any(t => t.Name == away.Name))
            group.AddTeamToGroup(away);

        if (!group.MatchesInGroup.Any(m => m.IdMatch == match.IdMatch))
            group.AddMatchToGroup(match);

        return match;
    }

    private Cruce CreateCruce(string name, int id)
    {
        var home = CreateTeam($"HomeCruce{id}");
        var away = CreateTeam($"AwayCruce{id}", "UEFA");
        var stadium = CreateStadium($"EstadioCruce{id}");
        var clock = new DateTimeMatchStub(new DateTime(2027, 1, 1));

        var match = new Match(
            id,
            new DateTime(2027, 6, 20, 18, 0, 0),
            stadium,
            home,
            away,
            'N',
            clock);

        match.SimulateResult(2, 1);

        return new Cruce(name, match, TournamentPhase.RoundOf32);
    }

    private static string ExportsFolder()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
    }

    private static string ExportUrlToFilePath(string exportUrl)
    {
        return Path.Combine(ExportsFolder(), Path.GetFileName(exportUrl));
    }

    private static void DeleteGeneratedExportFiles()
    {
        var folder = ExportsFolder();

        if (!Directory.Exists(folder))
            return;

        foreach (var file in Directory.GetFiles(folder))
        {
            var fileName = Path.GetFileName(file);

            if (fileName.StartsWith("fixture_", StringComparison.OrdinalIgnoreCase) ||
                fileName.StartsWith("logs_", StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(file);
            }
        }
    }

    private static void AssertExportFileExists(string exportUrl, string expectedExtension)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(exportUrl));
        StringAssert.StartsWith(exportUrl, "/exports/");
        StringAssert.EndsWith(exportUrl, expectedExtension);

        var filePath = ExportUrlToFilePath(exportUrl);
        Assert.IsTrue(File.Exists(filePath), $"No existe el archivo exportado: {filePath}");
    }

    [TestMethod]
    public void Constructor_WithNullUserQueryService_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new ExportService(
                null!,
                _logService,
                _fixtureRepository,
                _matchRepository,
                _crucesRepository));
    }

    [TestMethod]
    public void Constructor_WithNullLogService_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new ExportService(
                _userQueryServiceStub,
                null!,
                _fixtureRepository,
                _matchRepository,
                _crucesRepository));
    }

    [TestMethod]
    public void Constructor_WithNullFixtureRepository_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new ExportService(
                _userQueryServiceStub,
                _logService,
                null!,
                _matchRepository,
                _crucesRepository));
    }

    [TestMethod]
    public void Constructor_WithNullMatchRepository_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new ExportService(
                _userQueryServiceStub,
                _logService,
                _fixtureRepository,
                null!,
                _crucesRepository));
    }

    [TestMethod]
    public void Constructor_WithNullCrucesRepository_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new ExportService(
                _userQueryServiceStub,
                _logService,
                _fixtureRepository,
                _matchRepository,
                null!));
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsAdministrator_ShouldReturnExportPath()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportFixtureToCsv("admin@test.com");

        AssertExportFileExists(result, ".csv");
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsJournalist_ShouldReturnExportPath()
    {
        AddUser("periodista@test.com", UserRole.Journalist);

        var result = _service.ExportFixtureToCsv("periodista@test.com");

        AssertExportFileExists(result, ".csv");
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsEditor_ShouldThrowUnauthorizedAccessException()
    {
        AddUser("editor@test.com", UserRole.Editor);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _service.ExportFixtureToCsv("editor@test.com"));
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserDoesNotExist_ShouldThrowInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            _service.ExportFixtureToCsv("missing@test.com"));
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithFinishedGroupMatch_ShouldWriteFinalizedMatch()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);
        CreateMatch(1, "Uruguay", "Argentina", 'A', 2, 1);

        var result = _service.ExportFixtureToCsv("admin@test.com");

        var content = File.ReadAllText(ExportUrlToFilePath(result));

        StringAssert.Contains(content, "Fase,Grupo,Fecha,Estadio,Equipo local,Goles local,Equipo visitante,Goles visitante,Estado");
        StringAssert.Contains(content, "Grupos");
        StringAssert.Contains(content, "Uruguay");
        StringAssert.Contains(content, "Argentina");
        StringAssert.Contains(content, "Finalizado");
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithPendingGroupMatch_ShouldWritePendingMatch()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);
        CreateMatch(1, "Uruguay", "Argentina", 'A');

        var result = _service.ExportFixtureToCsv("admin@test.com");

        var content = File.ReadAllText(ExportUrlToFilePath(result));

        StringAssert.Contains(content, "Pendiente");
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithCruces_ShouldWriteCruces()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var cruce = CreateCruce("Cruce A1", 50);
        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        var result = _service.ExportFixtureToCsv("admin@test.com");

        var content = File.ReadAllText(ExportUrlToFilePath(result));

        StringAssert.Contains(content, "Cruce A1");
        StringAssert.Contains(content, "HomeCruce50");
        StringAssert.Contains(content, "AwayCruce50");
        StringAssert.Contains(content, "Finalizado");
    }

    [TestMethod]
    public void ExportFixtureToCsv_WithCommaInTeamName_ShouldEscapeCsvValue()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);
        CreateMatch(1, "Uruguay, Oficial", "Argentina", 'A', 2, 1);

        var result = _service.ExportFixtureToCsv("admin@test.com");

        var content = File.ReadAllText(ExportUrlToFilePath(result));

        StringAssert.Contains(content, "\"Uruguay, Oficial\"");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WhenUserIsAdministrator_ShouldReturnExportPath()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportFixtureToXlsx("admin@test.com");

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WhenUserIsJournalist_ShouldReturnExportPath()
    {
        AddUser("periodista@test.com", UserRole.Journalist);

        var result = _service.ExportFixtureToXlsx("periodista@test.com");

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WhenUserIsEditor_ShouldThrowUnauthorizedAccessException()
    {
        AddUser("editor@test.com", UserRole.Editor);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _service.ExportFixtureToXlsx("editor@test.com"));
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WithGroupMatch_ShouldCreateFile()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);
        CreateMatch(1, "Uruguay", "Argentina", 'A', 2, 1);

        var result = _service.ExportFixtureToXlsx("admin@test.com");

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WithCruces_ShouldCreateFile()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var cruce = CreateCruce("Cruce A1", 60);
        _crucesRepository.SetCruces(new List<Cruce> { cruce });

        var result = _service.ExportFixtureToXlsx("admin@test.com");

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenUserIsAdministrator_ShouldReturnExportPath()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportLogsToCsv(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        AssertExportFileExists(result, ".csv");
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenUserIsJournalist_ShouldReturnExportPath()
    {
        AddUser("periodista@test.com", UserRole.Journalist);

        var result = _service.ExportLogsToCsv(
            "periodista@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        AssertExportFileExists(result, ".csv");
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenUserIsEditor_ShouldThrowUnauthorizedAccessException()
    {
        AddUser("editor@test.com", UserRole.Editor);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _service.ExportLogsToCsv(
                "editor@test.com",
                DateTime.MinValue,
                DateTime.MaxValue));
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenFromDateIsAfterToDate_ShouldThrowArgumentException()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ExportLogsToCsv(
                "admin@test.com",
                new DateTime(2026, 6, 10),
                new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void ExportLogsToCsv_WithLogsInRange_ShouldWriteLogs()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _logService.Register(
            "user@test.com",
            "CategoryA",
            "ActionA",
            "DetailA");

        var result = _service.ExportLogsToCsv(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        var content = File.ReadAllText(ExportUrlToFilePath(result));

        StringAssert.Contains(content, "Timestamp,UserEmail,Category,Action,Detail");
        StringAssert.Contains(content, "CategoryA");
        StringAssert.Contains(content, "ActionA");
        StringAssert.Contains(content, "DetailA");
    }

    [TestMethod]
    public void ExportLogsToCsv_WithCommaAndQuotesInDetail_ShouldEscapeCsvValue()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _logService.Register(
            "user@test.com",
            "Export",
            "FileExported",
            "Detalle, con \"comillas\"");

        var result = _service.ExportLogsToCsv(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        var content = File.ReadAllText(ExportUrlToFilePath(result));

        StringAssert.Contains(content, "\"Detalle, con \"\"comillas\"\"\"");
    }

    [TestMethod]
    public void ExportLogsToXlsx_WhenUserIsAdministrator_ShouldReturnExportPath()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        var result = _service.ExportLogsToXlsx(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportLogsToXlsx_WhenUserIsJournalist_ShouldReturnExportPath()
    {
        AddUser("periodista@test.com", UserRole.Journalist);

        var result = _service.ExportLogsToXlsx(
            "periodista@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportLogsToXlsx_WhenUserIsEditor_ShouldThrowUnauthorizedAccessException()
    {
        AddUser("editor@test.com", UserRole.Editor);

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _service.ExportLogsToXlsx(
                "editor@test.com",
                DateTime.MinValue,
                DateTime.MaxValue));
    }

    [TestMethod]
    public void ExportLogsToXlsx_WhenFromDateIsAfterToDate_ShouldThrowArgumentException()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        Assert.ThrowsException<ArgumentException>(() =>
            _service.ExportLogsToXlsx(
                "admin@test.com",
                new DateTime(2026, 6, 10),
                new DateTime(2026, 6, 1)));
    }

    [TestMethod]
    public void ExportLogsToXlsx_WithLogsInRange_ShouldCreateFile()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _logService.Register(
            "user@test.com",
            "CategoryA",
            "ActionA",
            "DetailA");

        var result = _service.ExportLogsToXlsx(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        AssertExportFileExists(result, ".xlsx");
    }

    [TestMethod]
    public void ExportFixtureToCsv_WhenUserIsAdministrator_ShouldRegisterFileExportedLog()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _service.ExportFixtureToCsv("admin@test.com");

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Export", logs[0].Category);
        Assert.AreEqual("FileExported", logs[0].Action);
    }

    [TestMethod]
    public void ExportFixtureToXlsx_WhenUserIsAdministrator_ShouldRegisterFileExportedLog()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _service.ExportFixtureToXlsx("admin@test.com");

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Export", logs[0].Category);
        Assert.AreEqual("FileExported", logs[0].Action);
    }

    [TestMethod]
    public void ExportLogsToCsv_WhenUserIsAdministrator_ShouldRegisterFileExportedLog()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _service.ExportLogsToCsv(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Export", logs[0].Category);
        Assert.AreEqual("FileExported", logs[0].Action);
    }

    [TestMethod]
    public void ExportLogsToXlsx_WhenUserIsAdministrator_ShouldRegisterFileExportedLog()
    {
        AddUser("admin@test.com", UserRole.SystemAdministrator);

        _service.ExportLogsToXlsx(
            "admin@test.com",
            DateTime.MinValue,
            DateTime.MaxValue);

        var logs = _logRepository.GetAll();

        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("admin@test.com", logs[0].UserEmail);
        Assert.AreEqual("Export", logs[0].Category);
        Assert.AreEqual("FileExported", logs[0].Action);
    }
}