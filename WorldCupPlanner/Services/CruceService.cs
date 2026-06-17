using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using DTOs.Cruces;
using IRepository;
using IServices.CrucesInterface;
using IServices;
using Services.Mappers;
using ISimulationEngineFactory = IServices.ISimulationEngineFactory;

namespace Services;

public class CruceService : ICruceEditService, ICruceQueryService
{
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;

    private readonly ICrucesRepository _repository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly TournamentConfig _config;
    private readonly IMatchRepository _matchRepository;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly IDateTimeMatch _clock;
    private readonly ISimulationEngineFactory _engineFactory;
    private readonly ITeamRepository _teamRepository;
    private readonly ILogService _logService;

    private int _nextMatchId = 1;

    public CruceService(
        ICrucesRepository repository,
        IStadiumRepository stadiumRepository,
        TournamentConfig config,
        IMatchRepository matchRepository,
        IFixtureRepository fixtureRepository,
        IDateTimeMatch clock,
        ISimulationEngineFactory engineFactory,
        ITeamRepository teamRepository,
        ILogService logService)
    {
        _repository = repository;
        _stadiumRepository = stadiumRepository;
        _config = config;
        _matchRepository = matchRepository;
        _fixtureRepository = fixtureRepository;
        _clock = clock;
        _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
        _teamRepository = teamRepository;
        _logService = logService;
    }

    public void GenerateRoundOf32()
    {
        try
        {
            var fixture = _fixtureRepository.GetFixture()
                          ?? throw new InvalidOperationException("No hay fixture generado.");

            fixture.LoadMatches(_matchRepository.GetAllMatches());

            if (!fixture.CanAdvanceToPhase(TournamentPhase.RoundOf32))
                throw new InvalidOperationException("Todos los partidos de la fase de grupos deben estar jugados antes de generar los cruce.");

            fixture.LockGroupMatches();

            List<Stadium> stadiums = _stadiumRepository.GetAllStadiums();
            int nextMatchId = _matchRepository.GetNextId();
            List<Cruce> cruces = GenerateRoundOf32Internal(fixture, stadiums, _config.PhaseDrawSeed, nextMatchId);

            _repository.RemoveCruces();
            foreach (var cruce in cruces)
                _matchRepository.AddMatch(cruce.Match);
            _repository.AddCruces(cruces);
        }
        catch (Exception ex)
        {
            _logService.Register("system", "Error", "GenerateRoundOf32", ex.Message);
            throw;
        }
    }

    public void GenerateNextRound()
    {
        List<Cruce> actuals = _repository.GetAllCruces();
        if (actuals == null || actuals.Count == 0)
            throw new InvalidOperationException("No hay cruces generados.");

        TournamentPhase lastPhase = actuals.Max(x => x.Phase);
        if (lastPhase == TournamentPhase.Final)
            throw new InvalidOperationException("Ya se encuentra en la fase final.");

        List<Cruce> lastRound = actuals.Where(x => x.Phase == lastPhase).ToList();
        if (lastRound.Any(c => c.GetWinner() == null))
            throw new InvalidOperationException("Todos los cruces de la ronda actual deben tener resultado antes de generar la siguiente.");

        int nextMatchId = _matchRepository.GetNextId();
        List<Cruce> news = GenerateNextRoundInternal(lastRound, _stadiumRepository.GetAllStadiums(), _config.PhaseDrawSeed, nextMatchId);

        foreach (var cruce in news)
            _matchRepository.AddMatch(cruce.Match);
        _repository.AddCruces(news);
    }

    public List<CruceResponse> GetAllCruces()
    {
        return _repository.GetAllCruces().Select(CruceMapper.ToResponse).ToList();
    }

    public void SimulateCruces()
    {
        var fixture = _fixtureRepository.GetFixture()
                      ?? throw new InvalidOperationException("No hay fixture generado.");

        ISimulationEngine engine = _engineFactory.Create(fixture.SimulationEngineName);
        SimulateCruces(engine);
    }

    public void SimulateCruces(ISimulationEngine engine)
    {
        var cruces = _repository.GetAllCruces();
        foreach (var cruce in cruces.Where(c => c.GetWinner() == null))
        {
            var match = cruce.Match;
            var (homeGoals, awayGoals) = CalculateResult(match, _config.SimulationSeed, engine);

            if (homeGoals == awayGoals)
            {
                var tiebreaker = new Random(_config.SimulationSeed + match.IdMatch * 31 + 1);
                if (tiebreaker.NextDouble() > 0.5) homeGoals++;
                else awayGoals++;
            }

            match.SimulateResult(homeGoals, awayGoals);
            _matchRepository.UpdateMatch(match);
            UpdateRankingsAfterMatch(match);
        }
    }

    private void UpdateRankingsAfterMatch(Match match)
    {
        if (!match.HomeGoals.HasValue || !match.AwayGoals.HasValue)
            return;

        var fase = match.GetTournamentPhase();
        int prevHome = match.GetHomeRanking();
        int prevAway = match.GetAwayRanking();

        match.RecalculateRankings(fase);

        _teamRepository.Update(match.HomeTeam);
        _teamRepository.Update(match.AwayTeam);

        _logService.Register(
            "system", "Ranking", "RankingUpdated",
            $"{match.GetHomeTeamName()}: {prevHome} -> {match.GetHomeRanking()}. " +
            $"{match.GetAwayTeamName()}: {prevAway} -> {match.GetAwayRanking()}.");
    }

    private (int homeGoals, int awayGoals) CalculateResult(Match match, int seed, ISimulationEngine engine)
    {
        var matchSeed = seed + match.IdMatch;
        return engine.SimulateMatch(match.GetHomeRanking(), match.GetAwayRanking(), matchSeed);
    }

    private List<Cruce> GenerateRoundOf32Internal(Fixture fixture, List<Stadium> stadiums, int seed, int startMatchId)
    {
        _nextMatchId = startMatchId;
        ValidateInputs(fixture.Groups.ToList(), stadiums);

        var teamToGroup = fixture.GetTeamToGroupMap();
        var first = fixture.GetGroupWinners();
        var second = fixture.GetGroupRunnersUp();
        var bestThirdParties = fixture.GetThirdPlacedTeams().Take(8).ToList();

        var cruces = new List<Cruce>();
        int globalMatchIndex = 0;

        cruces.AddRange(PairAndCreateMatches(
            first.Take(8).ToList(), bestThirdParties,
            stadiums, seed, "A", TournamentPhase.RoundOf32,
            ref globalMatchIndex, teamToGroup));

        cruces.AddRange(PairAndCreateMatches(
            first.Skip(8).Take(4).ToList(), second.TakeLast(4).Reverse().ToList(),
            stadiums, seed + 1, "B", TournamentPhase.RoundOf32,
            ref globalMatchIndex, teamToGroup));

        List<Team> remainingRunnersUp = new List<Team>();
        if (second.Count > 4)
            for (int i = 0; i < second.Count - 4; i++)
                remainingRunnersUp.Add(second[i]);

        if (remainingRunnersUp.Count >= 2)
        {
            cruces.AddRange(PairAndCreateMatches(
                remainingRunnersUp.Take(remainingRunnersUp.Count / 2).ToList(),
                remainingRunnersUp.Skip(remainingRunnersUp.Count / 2).ToList(),
                stadiums, seed + 2, "B", TournamentPhase.RoundOf32,
                ref globalMatchIndex, teamToGroup, startMatchNumber: 5));
        }

        foreach (var cruce in cruces)
            fixture.AddEliminatoryMatch(cruce.Match);

        return cruces;
    }

    private List<Cruce> GenerateNextRoundInternal(List<Cruce> previousCruces, List<Stadium> stadium, int seed, int startMatchId)
    {
        _nextMatchId = startMatchId;
        if (previousCruces == null || previousCruces.Count == 0)
            throw new ArgumentException("Se requieren cruces anteriores para generar la siguiente ronda.");

        DateTime dateLastMatch = previousCruces.Max(c => c.GetMatchDate());

        if (previousCruces[0].Phase == TournamentPhase.SemiFinals)
            return GenerateSemiFinalsOutput(previousCruces, stadium, dateLastMatch);

        var nextPhase = previousCruces[0].GetNextPhase();

        if (nextPhase == TournamentPhase.RoundOf16)
            return CreateRoundOf16FromBrackets(previousCruces, stadium, nextPhase, dateLastMatch);

        var winners = ObtainWinners(previousCruces);
        return CreateMatchesConsecutive(winners, stadium, nextPhase, dateLastMatch);
    }

    private List<Cruce> PairAndCreateMatches(
        List<Team> list1,
        List<Team> list2,
        List<Stadium> stadiums,
        int seed,
        string matchPrefix,
        TournamentPhase phase,
        ref int globalMatchIndex,
        Dictionary<string, char>? matchToGroup = null,
        int startMatchNumber = 1)
    {
        var shuffledList1 = _config.Shuffle(list1, seed);
        var shuffledList2 = _config.Shuffle(list2, seed + 7);
        var availableTeams = new List<Team>(shuffledList2);
        var cruces = new List<Cruce>();
        int matchNumber = startMatchNumber;

        foreach (var team1 in shuffledList1)
        {
            Team? pairedTeam = null;
            int pairedIndex = -1;

            for (int i = 0; i < availableTeams.Count; i++)
            {
                bool sameGroup = matchToGroup != null && AreFromSameGroup(team1, availableTeams[i], matchToGroup);
                if (!sameGroup)
                {
                    pairedTeam = availableTeams[i];
                    pairedIndex = i;
                    break;
                }
            }

            if (pairedTeam == null && availableTeams.Count > 0)
            {
                pairedTeam = availableTeams[0];
                pairedIndex = 0;
            }

            if (pairedTeam == null)
                throw new InvalidOperationException($"No se pudo emparejar al equipo {team1.Name}.");

            var stadium = stadiums[globalMatchIndex % stadiums.Count];
            var match = CreateMatch(team1, pairedTeam, stadium, globalMatchIndex, stadiums.Count);
            cruces.Add(new Cruce($"{matchPrefix}{matchNumber}", match, phase));
            availableTeams.RemoveAt(pairedIndex);
            globalMatchIndex++;
            matchNumber++;
        }
        return cruces;
    }

    private static bool AreFromSameGroup(Team team1, Team team2, Dictionary<string, char> teamToGroup)
    {
        if (!teamToGroup.TryGetValue(team1.Name, out var grupo1)) return false;
        if (!teamToGroup.TryGetValue(team2.Name, out var grupo2)) return false;
        return grupo1 == grupo2;
    }

    private List<Cruce> CreateRoundOf16FromBrackets(List<Cruce> roundOf32Matches, List<Stadium> stadiums, TournamentPhase phase, DateTime lastDate)
    {
        var aBracket = roundOf32Matches
            .Where(c => c.Name.StartsWith("A"))
            .OrderBy(c => c.Name)
            .ToList();
        var bBracket = roundOf32Matches
            .Where(c => c.Name.StartsWith("B"))
            .OrderBy(c => c.Name)
            .ToList();

        if (aBracket.Count != bBracket.Count)
            throw new InvalidOperationException("Los brackets A y B deben tener la misma cantidad de cruces para generar octavos.");

        var cruces = new List<Cruce>();
        int globalMatchIndex = 0;

        for (int i = 0; i < aBracket.Count; i++)
        {
            var winnerA = (Team?)aBracket[i].GetWinner()
                           ?? throw new InvalidOperationException($"El cruce {aBracket[i].Name} no tiene ganador.");
            var winnerB = (Team?)bBracket[i].GetWinner()
                           ?? throw new InvalidOperationException($"El cruce {bBracket[i].Name} no tiene ganador.");

            var stadium = stadiums[globalMatchIndex % stadiums.Count];
            var match = CreateMatch(winnerA, winnerB, stadium, globalMatchIndex, stadiums.Count, lastDate);
            cruces.Add(new Cruce($"C{i + 1}", match, phase));
            globalMatchIndex++;
        }
        return cruces;
    }

    private List<Cruce> CreateMatchesConsecutive(List<Team> winners, List<Stadium> stadiums, TournamentPhase phase, DateTime lastDate)
    {
        if (winners.Count < 2)
            throw new ArgumentException("Se requieren al menos 2 ganadores para crear cruces.");

        string prefix = phase switch
        {
            TournamentPhase.QuarterFinals => "D",
            TournamentPhase.SemiFinals => "S",
            TournamentPhase.Final => "F",
            TournamentPhase.ThirdPlace => "TP",
            _ => "X"
        };

        var cruces = new List<Cruce>();
        int globalMatchIndex = 0;
        int cruceNumber = 1;

        for (int i = 0; i + 1 < winners.Count; i += 2)
        {
            var stadium = stadiums[globalMatchIndex % stadiums.Count];
            var match = CreateMatch(winners[i], winners[i + 1], stadium, globalMatchIndex, stadiums.Count, lastDate);
            cruces.Add(new Cruce($"{prefix}{cruceNumber}", match, phase));
            globalMatchIndex++;
            cruceNumber++;
        }
        return cruces;
    }

    private Match CreateMatch(Team homeTeam, Team awayTeam, Stadium stadium, int globalMatchIndex, int stadiumCount, DateTime? lastMatchDate = null)
    {
        int slotIndex = globalMatchIndex / stadiumCount;
        int slotEnDia = slotIndex % _config.MaxMatchesPerDay;
        int diaOffset = slotIndex / _config.MaxMatchesPerDay;
        int hour = StartHour + slotEnDia * HoursBetweenMatches;

        DateTime startDate = lastMatchDate.HasValue
            ? lastMatchDate.Value.Date.AddDays(_config.DaysBetweenMatchdays)
            : _config.TournamentStartDate.AddDays(21).Date;

        var matchDate = startDate.AddDays(diaOffset).AddHours(hour);
        return new Match(_nextMatchId++, matchDate, stadium, homeTeam, awayTeam, 'N', _clock);
    }

    private static List<Team> ObtainWinners(List<Cruce> cruces)
    {
        var winners = new List<Team>();
        foreach (var cruce in cruces)
        {
            var winner = (Team?)cruce.GetWinner();
            if (winner == null)
                throw new InvalidOperationException($"El cruce {cruce.Name} no tiene un ganador definido.");
            winners.Add(winner);
        }
        return winners;
    }

    private List<Cruce> GenerateSemiFinalsOutput(List<Cruce> semiFinals, List<Stadium> stadiums, DateTime baseDate)
    {
        if (semiFinals.Count != 2)
            throw new InvalidOperationException("Se esperan exactamente 2 semifinales.");

        var winners = ExtractTeamsFromMatches(semiFinals, c => (Team?)c.GetWinner());
        var losers = ExtractTeamsFromMatches(semiFinals, c => (Team?)c.GetLoser());

        int matchIndex = 0;

        var thirdPlaceStadium = stadiums[matchIndex % stadiums.Count];
        var thirdPlaceMatch = CreateMatch(losers[0], losers[1], thirdPlaceStadium, matchIndex, stadiums.Count, baseDate);
        var thirdPlaceCruce = new Cruce("TP1", thirdPlaceMatch, TournamentPhase.ThirdPlace);
        matchIndex++;

        var finalStadium = stadiums[matchIndex % stadiums.Count];
        var finalMatch = CreateMatch(winners[0], winners[1], finalStadium, matchIndex, stadiums.Count, baseDate);
        var finalCruce = new Cruce("F1", finalMatch, TournamentPhase.Final);

        return new List<Cruce> { thirdPlaceCruce, finalCruce };
    }

    private static List<Team> ExtractTeamsFromMatches(List<Cruce> cruces, Func<Cruce, Team?> teamExtractor)
    {
        var teams = new List<Team>();
        foreach (var cruce in cruces)
        {
            var team = teamExtractor(cruce);
            if (team == null)
                throw new InvalidOperationException($"El cruce {cruce.Name} no tiene un participante definido.");
            teams.Add(team);
        }
        return teams;
    }

    private static void ValidateInputs(List<Group> groups, List<Stadium> stadiums)
    {
        if (groups == null) throw new ArgumentNullException(nameof(groups));
        if (stadiums == null) throw new ArgumentNullException(nameof(stadiums));
        if (groups.Count == 0) throw new ArgumentException("La lista de grupos no puede estar vacía.");
        if (stadiums.Count == 0) throw new ArgumentException("Se requiere al menos un estadio para generar cruces.");
        if (groups.Count != 12) throw new ArgumentException("El torneo requiere exactamente 12 grupos (A-L).");
    }
}
