using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Match
{
    private const char MinGroup = 'A';
    private const char MaxGroup = 'L';
    private const char EliminatoryGroup = 'N';
    private const int MinGoals = 0;

    private int _idMatch;
    private DateTime _dateMatch;
    private Stadium _stadium = null!;
    private Team _homeTeam = null!;
    private Team _awayTeam = null!;
    private char _group;
    private int? _homeGoals;
    private int? _awayGoals;
    private Team? _winnerMatch;
    private Team? _loserMatch;
    private IDateTimeMatch _clock = null!;
    private bool _isLocked;
    private readonly List<MatchIncident> _incidents = new();

    public int IdMatch => _idMatch;

    public DateTime DateMatch
    {
        get => _dateMatch;
        set
        {
            EnsureValidDate(value);
            _dateMatch = value;
        }
    }

    public Stadium Stadium
    {
        get => _stadium;
        private set
        {
            EnsureValidStadium(value);
            _stadium = value;
        }
    }

    public Team HomeTeam
    {
        get => _homeTeam;
        private set
        {
            EnsureValidHomeTeam(value);
            _homeTeam = value;
        }
    }

    public Team AwayTeam
    {
        get => _awayTeam;
        private set
        {
            EnsureValidAwayTeam(value);
            _awayTeam = value;
        }
    }

    public char Group
    {
        get => _group;
        private set
        {
            EnsureValidGroup(value);
            _group = char.ToUpper(value);
        }
    }

    public bool IsLocked
    {
        get => _isLocked;
        private set => _isLocked = value;
    }

    public int? HomeGoals
    {
        get => _homeGoals;
        private set => _homeGoals = value;
    }

    public int? AwayGoals
    {
        get => _awayGoals;
        private set => _awayGoals = value;
    }

    public Team? WinnerMatch => _winnerMatch;
    public Team? LoserMatch => _loserMatch;
    public IReadOnlyList<MatchIncident> Incidents => _incidents;

    private Match()
    {
    }

    public Match(
        int id,
        DateTime dateMatch,
        Stadium stadium,
        Team homeTeam,
        Team awayTeam,
        char group,
        IDateTimeMatch clock)
    {
        SetClock(clock);
        Stadium = stadium;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        EnsureDifferentTeams();
        Group = group;
        DateMatch = dateMatch;
        _idMatch = id;
    }

    public void SetClock(IDateTimeMatch clock)
    {
        EnsureValidClock(clock);
        _clock = clock;
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void SetMatchResult(int homeGoals, int awayGoals)
    {
        EnsureMatchWasPlayed();
        EnsureResultWasNotSet();
        ApplyResult(homeGoals, awayGoals);
    }

    public void SimulateResult(int homeGoals, int awayGoals)
    {
        ApplyResult(homeGoals, awayGoals);
    }

    public void UpdateDate(DateTime newDate)
    {
        EnsureGroupPhaseIsEditable();
        DateMatch = newDate;
    }

    public void UpdateStadium(Stadium newStadium)
    {
        EnsureGroupPhaseIsEditable();
        Stadium = newStadium;
    }

    public void AddIncident(MatchIncident incident)
    {
        EnsureIncidentIsNotNull(incident);
        _incidents.Add(incident);
    }

    public void ClearCardIncidents()
    {
        _incidents.RemoveAll(incident =>
            incident.Type == IncidentType.YellowCard ||
            incident.Type == IncidentType.RedCard);
    }

    private void ApplyResult(int homeGoals, int awayGoals)
    {
        EnsureValidGoals(homeGoals, awayGoals);
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        SetWinnerFromResult(homeGoals, awayGoals);
        SetLoserFromResult(homeGoals, awayGoals);
    }

    private void EnsureValidClock(IDateTimeMatch clock)
    {
        if (clock == null)
            throw new ArgumentNullException(nameof(clock), "El reloj no puede ser vacío.");
    }

    private void EnsureDifferentTeams()
    {
        if (ReferenceEquals(HomeTeam, AwayTeam) || HomeTeam.Name == AwayTeam.Name)
            throw new ArgumentException("El equipo local y el equipo visitante deben ser diferentes.");
    }

    private void EnsureMatchWasPlayed()
    {
        if (_clock.Now.ToUniversalTime() < DateMatch.ToUniversalTime())
            throw new InvalidOperationException("El partido todavía no se jugó.");
    }

    private void EnsureResultWasNotSet()
    {
        if (HomeGoals.HasValue || AwayGoals.HasValue)
            throw new InvalidOperationException("El resultado ya fue seteado.");
    }

    private void EnsureValidGoals(int homeGoals, int awayGoals)
    {
        if (homeGoals < MinGoals || awayGoals < MinGoals)
            throw new ArgumentException("Los goles no pueden ser negativos.");
    }

    private void SetWinnerFromResult(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals)
            _winnerMatch = HomeTeam;
        else if (homeGoals < awayGoals)
            _winnerMatch = AwayTeam;
        else
            _winnerMatch = null;
    }

    private void SetLoserFromResult(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals)
            _loserMatch = AwayTeam;
        else if (homeGoals < awayGoals)
            _loserMatch = HomeTeam;
        else
            _loserMatch = null;
    }

    private void EnsureValidDate(DateTime dateMatch)
    {
        if (_clock == null)
            throw new InvalidOperationException("El reloj no fue configurado.");

        if (dateMatch.ToUniversalTime() < _clock.Now.ToUniversalTime())
            throw new ArgumentException("La fecha no puede ser pasada.");
    }

    private void EnsureValidStadium(Stadium stadium)
    {
        if (stadium == null)
            throw new ArgumentNullException(nameof(stadium), "El estadio no puede ser vacío.");
    }

    private void EnsureValidHomeTeam(Team homeTeam)
    {
        if (homeTeam == null)
            throw new ArgumentNullException(nameof(homeTeam), "El equipo local no puede ser vacío.");
    }

    private void EnsureValidAwayTeam(Team awayTeam)
    {
        if (awayTeam == null)
            throw new ArgumentNullException(nameof(awayTeam), "El equipo visitante no puede ser vacío.");
    }

    private void EnsureValidGroup(char group)
    {
        if (group == '\0')
            throw new ArgumentException("El grupo no puede ser vacío.", nameof(group));

        char normalizedGroup = char.ToUpper(group);

        if (normalizedGroup == EliminatoryGroup)
            return;

        if (normalizedGroup < MinGroup || normalizedGroup > MaxGroup)
            throw new ArgumentException($"El grupo debe ser una letra entre la {MinGroup} y la {MaxGroup}.");
    }

    private void EnsureGroupPhaseIsEditable()
    {
        if (IsLocked)
            throw new InvalidOperationException(
                "No se puede editar un partido de fase de grupos una vez generados los cruces.");
    }

    private static void EnsureIncidentIsNotNull(MatchIncident incident)
    {
        if (incident == null)
            throw new ArgumentNullException(nameof(incident));
    }
}