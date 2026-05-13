using DTOs;
using Domain;
using DomainTests.Stubs;
using IServices;
using Repository;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class MatchServiceTests
{
    private MatchService _service;
    private MatchRepository _matchRepository;
    private TeamRepositoryStub _teamRepository;
    private StadiumRepositoryStub _stadiumRepository;
    private DateTimeMatchStub _clock;
    private ISimulationService _simulationService;
    private FixtureRepository _fixtureRepository;
    private TournamentConfig _config;

    [TestInitialize]
    public void Setup()
    {
        _matchRepository = new MatchRepository();
        _teamRepository = new TeamRepositoryStub();
        _stadiumRepository = new StadiumRepositoryStub();
        _clock = new DateTimeMatchStub(new DateTime(2027, 1, 1));
        _simulationService = new SimulationServiceStub();
        _fixtureRepository = new FixtureRepository();
        _config = new TournamentConfig();

        _service = new MatchService(
            _matchRepository,
            _teamRepository,
            _stadiumRepository,
            _clock,
            _simulationService,
            _fixtureRepository,
            _config
        );
    }

    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("CONMEBOL"), 1900);
    }

    private Stadium CreateStadium(string name)
    {
        return new Stadium(name, "Montevideo", null, 60000);
    }

    private MatchRequest CreateValidRequest(DateTime date)
    {
        return new MatchRequest
        {
            dateMatch = date,
            stadiumName = "Centenario",
            homeTeamName = "Uruguay",
            awayTeamName = "Argentina",
            group = "A"
        };
    }

    private void AddDefaultTeamsAndStadium()
    {
        _teamRepository.Add(CreateTeam("Uruguay"));
        _teamRepository.Add(CreateTeam("Argentina"));
        _stadiumRepository.AddStadium(CreateStadium("Centenario"));
    }

    [TestMethod]
    public void Create_WithValidRequest_ShouldCreateMatch()
    {
        AddDefaultTeamsAndStadium();
        var response = _service.CreateMatch(CreateValidRequest(_clock.Now));
        Assert.AreEqual(1, response.idMatch);
        Assert.AreEqual("Centenario", response.stadiumName);
        Assert.AreEqual("Uruguay", response.homeTeamName);
        Assert.AreEqual("Argentina", response.awayTeamName);
        Assert.AreEqual('A', response.group);
    }

    [TestMethod]
    public void Create_WithNullRequest_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.CreateMatch(null));
    }

    [TestMethod]
    public void Create_WithValidRequest_ShouldStoreMatchInRepository()
    {
        AddDefaultTeamsAndStadium();
        _service.CreateMatch(CreateValidRequest(_clock.Now));
        Assert.AreEqual(1, _matchRepository.GetAllMatches().Count);
    }

    [TestMethod]
    public void Create_WithEmptyGroup_ShouldThrowArgumentException()
    {
        var request = CreateValidRequest(_clock.Now);
        request.group = "";
        Assert.ThrowsException<ArgumentException>(() => _service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithNonExistingTeam_ShouldThrowKeyNotFoundException()
    {
        _teamRepository.Add(CreateTeam("Uruguay"));
        _stadiumRepository.AddStadium(CreateStadium("Centenario"));
        var request = CreateValidRequest(_clock.Now);
        Assert.ThrowsException<KeyNotFoundException>(() => _service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithNonExistingStadium_ShouldThrowKeyNotFoundException()
    {
        _teamRepository.Add(CreateTeam("Uruguay"));
        _teamRepository.Add(CreateTeam("Argentina"));
        var request = CreateValidRequest(_clock.Now);
        Assert.ThrowsException<KeyNotFoundException>(() => _service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithSameHomeAndAwayTeam_ShouldThrowArgumentException()
    {
        AddDefaultTeamsAndStadium();
        var request = CreateValidRequest(_clock.Now);
        request.awayTeamName = "Uruguay";
        Assert.ThrowsException<ArgumentException>(() => _service.CreateMatch(request));
    }

    [TestMethod]
    public void Create_WithPastDate_ShouldThrowArgumentException()
    {
        AddDefaultTeamsAndStadium();
        var request = CreateValidRequest(new DateTime(2026, 6, 1));
        Assert.ThrowsException<ArgumentException>(() => _service.CreateMatch(request));
    }

    [TestMethod]
    public void SetResult_WithValidData_ShouldUpdateMatchResult()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        _service.SetMatchResult(created.idMatch, 2, 1);
        var match = _matchRepository.GetMatchById(created.idMatch);
        Assert.AreEqual(2, match.HomeGoals);
        Assert.AreEqual(1, match.AwayGoals);
    }

    [TestMethod]
    public void SetResult_WithNegativeGoals_ShouldThrowArgumentException()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        Assert.ThrowsException<ArgumentException>(() => _service.SetMatchResult(created.idMatch, -1, 0));
    }

    [TestMethod]
    public void SetResult_WhenMatchNotPlayedYet_ShouldThrowInvalidOperationException()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(new DateTime(2027, 6, 10)));
        Assert.ThrowsException<InvalidOperationException>(() => _service.SetMatchResult(created.idMatch, 2, 1));
    }

    [TestMethod]
    public void SetResult_WhenResultAlreadySet_ShouldThrowInvalidOperationException()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        _service.SetMatchResult(created.idMatch, 1, 0);
        Assert.ThrowsException<InvalidOperationException>(() => _service.SetMatchResult(created.idMatch, 2, 0));
    }

    [TestMethod]
    public void GetById_WithExistingMatch_ShouldReturnMatchResponse()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        var result = _service.GetMatchById(created.idMatch);
        Assert.AreEqual(created.idMatch, result.idMatch);
        Assert.AreEqual("Centenario", result.stadiumName);
        Assert.AreEqual("Uruguay", result.homeTeamName);
        Assert.AreEqual("Argentina", result.awayTeamName);
    }

    [TestMethod]
    public void GetById_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        Assert.ThrowsException<KeyNotFoundException>(() => _service.GetMatchById(99));
    }

    [TestMethod]
    public void GetAll_WhenThereAreNoMatches_ShouldReturnEmptyList()
    {
        var result = _service.GetAllMatches();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAll_WithMatches_ShouldReturnAllMatchResponses()
    {
        AddDefaultTeamsAndStadium();
        _teamRepository.Add(CreateTeam("Brasil"));
        _teamRepository.Add(CreateTeam("Peru"));
        _service.CreateMatch(CreateValidRequest(_clock.Now));
        _service.CreateMatch(new MatchRequest
        {
            dateMatch = _clock.Now,
            stadiumName = "Centenario",
            homeTeamName = "Brasil",
            awayTeamName = "Peru",
            group = "B"
        });
        var result = _service.GetAllMatches();
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Uruguay", result[0].homeTeamName);
        Assert.AreEqual("Brasil", result[1].homeTeamName);
    }

    [TestMethod]
    public void UpdateDate_WithValidDate_ShouldUpdateMatchDate()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        var newDate = new DateTime(2027, 6, 5);
        _service.UpdateMatchDate(created.idMatch, newDate);
        var updated = _service.GetMatchById(created.idMatch);
        Assert.AreEqual(newDate, updated.dateMatch);
    }

    [TestMethod]
    public void UpdateDate_WithPastDate_ShouldThrowArgumentException()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        var pastDate = new DateTime(2026, 6, 1);
        Assert.ThrowsException<ArgumentException>(() => _service.UpdateMatchDate(created.idMatch, pastDate));
    }

    [TestMethod]
    public void UpdateStadium_WithValidData_ShouldUpdateStadium()
    {
        AddDefaultTeamsAndStadium();
        _stadiumRepository.AddStadium(CreateStadium("Maracana"));
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        _service.UpdateMatchStadium(created.idMatch, "Maracana");
        var updated = _service.GetMatchById(created.idMatch);
        Assert.AreEqual("Maracana", updated.stadiumName);
    }

    [TestMethod]
    public void UpdateStadium_WithNonExistingStadium_ShouldThrowKeyNotFoundException()
    {
        AddDefaultTeamsAndStadium();
        var created = _service.CreateMatch(CreateValidRequest(_clock.Now));
        Assert.ThrowsException<KeyNotFoundException>(() => _service.UpdateMatchStadium(created.idMatch, "Maracana"));
    }
    
    [TestMethod]
    public void UpdateMatchDate_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        Assert.ThrowsException<KeyNotFoundException>(() =>
            _service.UpdateMatchDate(99, _clock.Now));
    }
    
    [TestMethod]
    public void UpdateMatchStadium_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        _stadiumRepository.AddStadium(CreateStadium("Maracana"));

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _service.UpdateMatchStadium(99, "Maracana"));
    }
    
    [TestMethod]
    public void SetMatchResult_WithNonExistingMatch_ShouldThrowKeyNotFoundException()
    {
        Assert.ThrowsException<KeyNotFoundException>(() =>
            _service.SetMatchResult(99, 1, 0));
    }
    
    [TestMethod]
    public void Create_WithInvalidGroup_ShouldThrowArgumentException()
    {
        AddDefaultTeamsAndStadium();

        var request = CreateValidRequest(_clock.Now);
        request.group = "M";

        Assert.ThrowsException<ArgumentException>(() =>
            _service.CreateMatch(request));
    }
    
    [TestMethod]
    public void Create_WithLowercaseGroup_ShouldCreateMatchWithUppercaseGroup()
    {
        AddDefaultTeamsAndStadium();

        var request = CreateValidRequest(_clock.Now);
        request.group = "a";

        var response = _service.CreateMatch(request);

        Assert.AreEqual('A', response.group);
    }
    
    [TestMethod]
    public void Create_WithNullGroup_ShouldThrowArgumentException()
    {
        AddDefaultTeamsAndStadium();

        var request = CreateValidRequest(_clock.Now);
        request.group = null!;

        Assert.ThrowsException<ArgumentException>(() =>
            _service.CreateMatch(request));
    }
}
