namespace Domain;

public class Match
{
    private const char MinGroup = 'A';
    private const char MaxGroup = 'L';
    private const int MinGoals = 0;

    private static int _idCounter = 0;

    private int _idMatch;
    private DateTime _dateMatch;
    private IStadium _stadium = null!;
    private ITeam _homeTeam = null!;
    private ITeam _awayTeam = null!;
    private char _group;
    private int? _homeGoals;
    private int? _awayGoals;
    private ITeam? _winnerMatch;
    private IDateTimeMatch _clock = null!;
    private bool _isLocked = false;
    public void Lock() => _isLocked = true;

    public int IdMatch => _idMatch;
    
    public void SetClock(IDateTimeMatch clock)
    {
        if (clock == null) throw new ArgumentNullException(nameof(clock));
        _clock = clock;
    }

    public DateTime DateMatch
    {
        get => _dateMatch;
        set
        {
            EnsureValidDate(value);
            _dateMatch = value;
        }
    }

    public IStadium Stadium
    {
        get => _stadium;
        set
        {
            EnsureValidStadium(value);
            _stadium = value;
        }
    }

    public ITeam HomeTeam
    {
        get => _homeTeam;
        private set
        {
            EnsureValidHomeTeam(value);
            _homeTeam = value;
        }
    }

    public ITeam AwayTeam
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
        set => _isLocked = value;
    }

    public int? HomeGoals => _homeGoals;
    public int? AwayGoals => _awayGoals;
    public ITeam? WinnerMatch => _winnerMatch;

    public Match(DateTime dateMatch, IStadium stadium, ITeam homeTeam, ITeam awayTeam, char group, IDateTimeMatch clock)
    {
        EnsureValidClock(clock);
        _clock = clock;
        
        Stadium = stadium;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        EnsureDifferentTeams();
        Group = group;
        DateMatch = dateMatch;
        _idMatch = ++_idCounter;
    }

    private void EnsureValidClock(IDateTimeMatch clock)
    {
        if (clock == null)
            throw new ArgumentNullException(nameof(clock), "El reloj no puede ser vacío.");
    }

    private void EnsureDifferentTeams()
    {
        if (HomeTeam.Name == AwayTeam.Name)
            throw new ArgumentException("El equipo local y el equipo visitante deben ser diferentes.");
    }

    public static void ResetIdCounter()
    {
        _idCounter = 0;
    }

    public void SetMatchResult(int homeGoals, int awayGoals)
    {
        EnsureMatchWasPlayed();
        EnsureResultWasNotSet();
        EnsureValidGoals(homeGoals, awayGoals);

        _homeGoals = homeGoals;
        _awayGoals = awayGoals;
        SetWinnerFromResult(homeGoals, awayGoals);
    }

    private void EnsureMatchWasPlayed()
    {
        if (_clock.Now < DateMatch)
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

    private void EnsureValidDate(DateTime dateMatch)
    {
        if (_clock == null)
            throw new InvalidOperationException("El reloj no fue configurado.");

        if (dateMatch < _clock.Now)
            throw new ArgumentException("La fecha no puede ser pasada.");
    }
    private void EnsureValidStadium(IStadium stadium)
    {
        if (stadium == null)
            throw new ArgumentNullException(nameof(stadium), "El estadio no puede ser vacío.");
    }

    private void EnsureValidHomeTeam(ITeam homeTeam)
    {
        if (homeTeam == null)
            throw new ArgumentNullException(nameof(homeTeam), "El equipo local no puede ser vacío.");
    }

    private void EnsureValidAwayTeam(ITeam awayTeam)
    {
        if (awayTeam == null)
            throw new ArgumentNullException(nameof(awayTeam), "El equipo visitante no puede ser vacío.");
    }

    private void EnsureValidGroup(char group)
    {
        if (group == '\0')
            throw new ArgumentNullException(nameof(group), "El grupo no puede ser vacío.");

        char groupToUpper = char.ToUpper(group);

        if (groupToUpper < MinGroup || groupToUpper > MaxGroup)
            throw new ArgumentException($"El grupo debe ser una letra entre la {MinGroup} y la {MaxGroup}.");
    }
    
    public void ForceWinner(ITeam winner)
    {
        if (winner == null)
            throw new ArgumentNullException(nameof(winner));
        if (winner.Name != HomeTeam.Name && winner.Name != AwayTeam.Name)
            throw new ArgumentException("El ganador debe ser uno de los equipos del partido.");
        _winnerMatch = winner;
    }
    
    private void EnsureGroupPhaseIsEditable()
    {
        if (IsLocked)
            throw new InvalidOperationException("No se puede editar un partido de fase de grupos una vez generados los cruces.");
    }
    public void SimulateResult(int homeGoals, int awayGoals)
    {
        EnsureResultWasNotSet();
        EnsureValidGoals(homeGoals, awayGoals);
        _homeGoals = homeGoals;
        _awayGoals = awayGoals;
        SetWinnerFromResult(homeGoals, awayGoals);
    }
}