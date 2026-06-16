using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Fixture;
using IRepository;
using IServices;
using IServices.FixtureInterfaces;
using Services.Mappers;

namespace Services;

public class FixtureService : IFixtureEditService, IFixtureQueryService
{
    private readonly IFixtureRepository _fixtureRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ICrucesRepository _crucesRepository;
    private readonly TournamentConfig _config;
    private readonly IDateTimeMatch _clock;

    private const int TeamsPerGroup = 4;
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;
    private const int GroupsPerBatch = 6;

    private int _nextMatchId = 1;

    public FixtureService(
        IFixtureRepository fixtureRepository,
        ITeamRepository teamRepository,
        IStadiumRepository stadiumRepository,
        IMatchRepository matchRepository,
        ICrucesRepository crucesRepository,
        TournamentConfig config,
        IDateTimeMatch clock)
    {
        _fixtureRepository = fixtureRepository;
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
        _matchRepository = matchRepository;
        _crucesRepository = crucesRepository;
        _config = config;
        _clock = clock;
    }

    public FixtureResponse GetFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return new FixtureResponse();

        var fixture = _fixtureRepository.GetFixture();
        fixture.CargarPartidos(_matchRepository.GetAllMatches());
        return fixture.ToEntity();
    }

    public Fixture GetFixtureEntity()
    {
        if (!_fixtureRepository.ContainsFixture())
            throw new InvalidOperationException("No hay fixture generado.");

        var fixture = _fixtureRepository.GetFixture();
        fixture.CargarPartidos(_matchRepository.GetAllMatches());
        return fixture;
    }

    public bool HasFixture()
    {
        try
        {
            ValidateExistingFixture();
            return _fixtureRepository.ContainsFixture();
        }
        catch
        {
            return false;
        }
    }

    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
    {
        var fixture = GetFixtureEntity();
        var group = fixture.Groups.FirstOrDefault(g => g.GroupLetter == groupLetter);
        if (group == null)
            throw new ArgumentException($"Grupo {groupLetter} no encontrado");

        return group.GetStandings()
            .Select(s => StandingMapper.ToResponse(s))
            .ToList();
    }

    public void GenerateFixture(string simulationEngineName = "Probabilistico")
    {
        _crucesRepository.RemoveCruces();
        _matchRepository.ClearMatches();

        if (_fixtureRepository.ContainsFixture())
            _fixtureRepository.RemoveFixture(_fixtureRepository.GetFixture());

        var teams = _teamRepository.GetAll();
        foreach (var team in teams)
        {
            team.ResetRankingToFifa();
            _teamRepository.Update(team);
        }

        _nextMatchId = _matchRepository.GetNextId();
        var fixture = GenerateGroupsStageFixture();

        foreach (var grupo in fixture.Groups)
        foreach (var partido in grupo.MatchesInGroup)
            _matchRepository.AddMatch(partido);

        fixture.SetSimulationEngine(simulationEngineName);
        _fixtureRepository.AddFixture(fixture);
    }

    private Fixture GenerateGroupsStageFixture()
    {
        var teams = _teamRepository.GetAll();
        var stadiums = _stadiumRepository.GetAllStadiums();

        if (teams.Count != 48)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");
        if (stadiums.Count < 4)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

        var fixture = new Fixture(_config);
        fixture.InitializeGroups();

        var orderedTeams = OrderTeamsWithTieBreak(teams);
        DistributeTeamsToGroups(orderedTeams, fixture.Groups.ToList());
        GenerateMatches(fixture, stadiums);

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        return fixture;
    }

    private List<Team> OrderTeamsWithTieBreak(List<Team> teams)
    {
        var result = new List<Team>();
        var groupedByRanking = teams
            .GroupBy(t => t.FifaRanking)
            .OrderByDescending(g => g.Key);

        foreach (var rankingGroup in groupedByRanking)
        {
            var tied = rankingGroup.ToList();
            var shuffled = tied.Count > 1 ? _config.Shuffle(tied, _config.SemillaFixture) : tied;
            result.AddRange(shuffled);
        }
        return result;
    }

    private void DistributeTeamsToGroups(List<Team> orderedTeams, List<Group> groups)
    {
        bool success = TryAssignAll(orderedTeams, 0, groups);
        if (!success)
            throw new InvalidOperationException("No se pudo distribuir los equipos respetando las restricciones de confederación.");
    }

    private bool TryAssignAll(List<Team> teams, int index, List<Group> groups)
    {
        if (index == teams.Count) return true;

        var team = teams[index];
        int roundRobinGroup = index % groups.Count;

        for (int offset = 0; offset < groups.Count; offset++)
        {
            int groupIndex = (roundRobinGroup + offset) % groups.Count;
            var group = groups[groupIndex];

            if (group.TeamsInGroup.Count >= TeamsPerGroup) continue;
            if (!group.CanAddTeam(team)) continue;

            group.AddTeamToGroup(team);
            if (TryAssignAll(teams, index + 1, groups)) return true;
            group.RemoveTeamFromGroup(team);
        }
        return false;
    }

    private void GenerateMatches(Fixture fixture, List<Stadium> stadiums)
    {
        var orderedStadiums = stadiums
            .OrderBy(s => NormalizeName(s.StadiumName))
            .ToList();

        var groups = fixture.Groups.ToList();
        int matchCounter = 0;

        foreach (var (groupIndex, group) in groups.Select((g, i) => (i, g)))
            matchCounter = GenerateGroupMatches(group, orderedStadiums, groupIndex, matchCounter);
    }

    private int GenerateGroupMatches(Group group, List<Stadium> orderedStadiums, int groupIndex, int matchCounter)
    {
        var teams = group.TeamsInGroup.ToList();
        var matchDates = CalculateMatchDatesForGroup(groupIndex);
        var matchups = GetGroupRoundRobinMatchups();

        for (int i = 0; i < matchups.Length; i++)
        {
            var stadium = orderedStadiums[matchCounter % orderedStadiums.Count];
            var match = new Match(
                _nextMatchId++,
                matchDates[i],
                stadium,
                teams[matchups[i].home],
                teams[matchups[i].away],
                group.GroupLetter,
                _clock);
            group.AddMatchToGroup(match);
            matchCounter++;
        }
        return matchCounter;
    }

    private static (int home, int away)[] GetGroupRoundRobinMatchups()
    {
        return new (int home, int away)[]
        {
            (0, 3), (1, 2),
            (0, 2), (1, 3),
            (0, 1), (2, 3),
        };
    }

    private DateTime[] CalculateMatchDatesForGroup(int groupIndex)
    {
        int batchIndex = groupIndex / GroupsPerBatch;
        int positionInBatch = groupIndex % GroupsPerBatch;

        DateTime round1Date = CalculateRoundDate(batchIndex, roundOffset: 0, positionInBatch);
        DateTime round2Date = CalculateRoundDate(batchIndex, roundOffset: _config.SeparacionEntreFechas, positionInBatch);
        DateTime round3Date = CalculateRoundDate(batchIndex, roundOffset: _config.SeparacionEntreFechas * 2, positionInBatch);

        return new[] { round1Date, round1Date, round2Date, round2Date, round3Date, round3Date };
    }

    private DateTime CalculateRoundDate(int batchIndex, int roundOffset, int positionInBatch)
    {
        int daysFromStart = batchIndex + roundOffset;
        int hour = StartHour + (positionInBatch % _config.MaxPartidosPorDia) * HoursBetweenMatches;
        return _config.FechaInicioTorneo.Date.AddDays(daysFromStart).AddHours(hour);
    }

    private void ValidateExistingFixture()
    {
        if (!_fixtureRepository.ContainsFixture())
            return;

        var fixture = GetFixtureEntity();

        if (!fixture.IsStillValid(
                _teamRepository.GetAll().Cast<ITeam>().ToList(),
                _stadiumRepository.GetAllStadiums().Cast<IStadium>().ToList()))
        {
            _fixtureRepository.RemoveFixture(fixture);
        }
    }

    private static string NormalizeName(string name)
    {
        var lower = name.ToLowerInvariant();
        var normalized = string.Concat(lower.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));
        return string.Join(" ", normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    public void DeleteFixture()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        _crucesRepository.RemoveCruces();
        _matchRepository.ClearMatches();
        _fixtureRepository.RemoveFixture(fixture);

        var teams = _teamRepository.GetAll();
        foreach (var team in teams)
        {
            team.ResetRankingToFifa();
            _teamRepository.Update(team);
        }
    }
}