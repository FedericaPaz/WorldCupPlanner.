using Domain.Interfaces;

namespace Domain.Entities;

public class Match
{
    private const char MinGroup = 'A';
    private const char MaxGroup = 'L';
    private const char EliminatoryGroup = 'N';
    private const int MinGoals = 0;

    private static int _idCounter;

    private int _idMatch;
    private DateTime _dateMatch;
    private IStadium _stadium = null!;
    private ITeam _homeTeam = null!;
    private ITeam _awayTeam = null!;
    private char _group;
    private int? _homeGoals;
    private int? _awayGoals;
    private ITeam? _winnerMatch;
    private ITeam? _loserMatch;
    private IDateTimeMatch _clock = null!;
    private bool _isLocked;

    public int IdMatch
    {
        get { return _idMatch; }
    }

    public DateTime DateMatch
    {
        get { return _dateMatch; }
        set
        {
            EnsureValidDate(value);
            _dateMatch = value;
        }
    }

    public IStadium Stadium
    {
        get { return _stadium; }
        set
        {
            EnsureValidStadium(value);
            _stadium = value;
        }
    }

    public ITeam HomeTeam
    {
        get { return _homeTeam; }
        private set
        {
            EnsureValidHomeTeam(value);
            _homeTeam = value;
        }
    }

    public ITeam AwayTeam
    {
        get { return _awayTeam; }
        private set
        {
            EnsureValidAwayTeam(value);
            _awayTeam = value;
        }
    }

    public char Group
    {
        get { return _group; }
        private set
        {
            EnsureValidGroup(value);
            _group = char.ToUpper(value);
        }
    }

    public bool IsLocked
    {
        get { return _isLocked; }
        private set { _isLocked = value; }
    }

    public int? HomeGoals
    {
        get { return _homeGoals; }
    }

    public int? AwayGoals
    {
        get { return _awayGoals; }
    }

    public ITeam? WinnerMatch
    {
        get { return _winnerMatch; }
    }

    public ITeam? LoserMatch
    {
        get { return _loserMatch; }
    }

    public Match(DateTime dateMatch, IStadium stadium, ITeam homeTeam, ITeam awayTeam, char group, IDateTimeMatch clock)
    {
        SetClock(clock);
        Stadium = stadium;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        EnsureDifferentTeams();
        Group = group;
        DateMatch = dateMatch;
        _idMatch = GenerateNextId();
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

    public static void ResetIdCounter()
    {
        _idCounter = 0;
    }

    public void SetMatchResult(int homeGoals, int awayGoals)
    {
        EnsureMatchWasPlayed();
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

    public void UpdateStadium(IStadium newStadium)
    {
        EnsureGroupPhaseIsEditable();
        Stadium = newStadium;
    }

    private static int GenerateNextId()
    {
        _idCounter++;
        return _idCounter;
    }

    private void ApplyResult(int homeGoals, int awayGoals)
    {
        EnsureResultWasNotSet();
        EnsureValidGoals(homeGoals, awayGoals);

        _homeGoals = homeGoals;
        _awayGoals = awayGoals;

        SetWinnerFromResult(homeGoals, awayGoals);
        SetLoserFromResult(homeGoals, awayGoals);
    }

    private void EnsureValidClock(IDateTimeMatch clock)
    {
        if (clock == null)
        {
            throw new ArgumentNullException(nameof(clock), "El reloj no puede ser vacío.");
        }
    }

    private void EnsureDifferentTeams()
    {
        if (ReferenceEquals(HomeTeam, AwayTeam))
        {
            throw new ArgumentException(
                "El equipo local y el equipo visitante deben ser diferentes.");
        }

        if (HomeTeam.Name == AwayTeam.Name)
        {
            throw new ArgumentException(
                "No se puede crear un partido con dos equipos con el mismo nombre.");
        }
    }

    private void EnsureMatchWasPlayed()
    {
        if (_clock.Now.ToUniversalTime() < DateMatch.ToUniversalTime())
        {
            throw new InvalidOperationException("El partido todavía no se jugó.");
        }
    }

    private void EnsureResultWasNotSet()
    {
        if (HomeGoals.HasValue || AwayGoals.HasValue)
        {
            throw new InvalidOperationException("El resultado ya fue seteado.");
        }
    }

    private void EnsureValidGoals(int homeGoals, int awayGoals)
    {
        if (homeGoals < MinGoals || awayGoals < MinGoals)
        {
            throw new ArgumentException("Los goles no pueden ser negativos.");
        }
    }

    private void SetWinnerFromResult(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals)
        {
            _winnerMatch = HomeTeam;
        }
        else if (homeGoals < awayGoals)
        {
            _winnerMatch = AwayTeam;
        }
        else
        {
            _winnerMatch = null;
        }
    }

    private void SetLoserFromResult(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals)
        {
            _loserMatch = AwayTeam;
        }
        else if (homeGoals < awayGoals)
        {
            _loserMatch = HomeTeam;
        }
        else
        {
            _loserMatch = null;
        }
    }

    private void EnsureValidDate(DateTime dateMatch)
    {
        if (_clock == null)
        {
            throw new InvalidOperationException("El reloj no fue configurado.");
        }

        if (dateMatch.ToUniversalTime() < _clock.Now.ToUniversalTime())
        {
            throw new ArgumentException("La fecha no puede ser pasada.");
        }
    }

    private void EnsureValidStadium(IStadium stadium)
    {
        if (stadium == null)
        {
            throw new ArgumentNullException(
                nameof(stadium),
                "El estadio no puede ser vacío.");
        }
    }

    private void EnsureValidHomeTeam(ITeam homeTeam)
    {
        if (homeTeam == null)
        {
            throw new ArgumentNullException(
                nameof(homeTeam),
                "El equipo local no puede ser vacío.");
        }
    }

    private void EnsureValidAwayTeam(ITeam awayTeam)
    {
        if (awayTeam == null)
        {
            throw new ArgumentNullException(
                nameof(awayTeam),
                "El equipo visitante no puede ser vacío.");
        }
    }

    private void EnsureValidGroup(char group)
    {
        if (group == '\0')
        {
            throw new ArgumentException(
                "El grupo no puede ser vacío.",
                nameof(group));
        }

        char normalizedGroup = char.ToUpper(group);

        if (normalizedGroup == EliminatoryGroup)
        {
            return;
        }

        if (normalizedGroup < MinGroup || normalizedGroup > MaxGroup)
        {
            throw new ArgumentException(
                $"El grupo debe ser una letra entre la {MinGroup} y la {MaxGroup}.");
        }
    }

    private void EnsureGroupPhaseIsEditable()
    {
        if (IsLocked)
        {
            throw new InvalidOperationException(
                "No se puede editar un partido de fase de grupos una vez generados los cruces.");
        }
    }
}