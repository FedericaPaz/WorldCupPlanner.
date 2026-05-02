namespace Domain;

public class Match
{
    private static int _idCounter = 0;

    private int _idMatch;
    private DateTime _dateMatch;
    private IStadium _stadium;
    private ITeam _homeTeam;
    private ITeam _awayTeam;
    private char _group;
    private int? _homeGoals;
    private int? _awayGoals;
    private ITeam? _winnerMatch;
    private IDateTimeMatch _clock;

    public int IdMatch => _idMatch;
    public DateTime DateMatch => _dateMatch;
    public IStadium Stadium => _stadium;
    public ITeam HomeTeam => _homeTeam;
    public ITeam AwayTeam => _awayTeam;
    public char Group => _group;
    public int? HomeGoals => _homeGoals;
    public int? AwayGoals => _awayGoals;
    public ITeam? WinnerMatch => _winnerMatch;


    public Match(DateTime dateMatch, IStadium stadium, ITeam homeTeam, ITeam awayTeam, char group, IDateTimeMatch clock)
    {
        ValidateClock(clock);
        ValidateDate(dateMatch, clock);
        ValidateStadium(stadium);
        ValidateTeams(homeTeam, awayTeam);
        ValidateGroup(group);

        _idMatch = ++_idCounter;
        _dateMatch = dateMatch;
        _stadium = stadium;
        _homeTeam = homeTeam;
        _awayTeam = awayTeam;
        _group = char.ToUpper(group);
        _clock = clock;
    }

    public static void ResetIdCounter()
    {
        _idCounter = 0;
    }
    
    private void ValidateGroup(char group)
    {
        if (group == '\0')  throw new ArgumentNullException("El grupo no puede ser vacio.");
        char groupToUpper = char.ToUpper(group);
        if (groupToUpper < 'A' || groupToUpper > 'L') throw new ArgumentException($"El grupo debe ser una letra entre la A y la L. Valor recibido: '{group}'.");
    }
    
    private void ValidateTeams(ITeam homeTeam, ITeam awayTeam)
    {
        if (homeTeam == null) throw new ArgumentNullException(nameof(homeTeam), "El equipo local no puede ser vacio.");
        if (awayTeam == null) throw new ArgumentNullException(nameof(awayTeam), "El equipo visitante no puede ser vacio.");
        if (homeTeam._teamName == awayTeam._teamName)
            throw new ArgumentException("El equipo local y el equipo visitante deben ser diferentes.");
    }

    private void ValidateStadium(IStadium stadium)
    {
        if (stadium == null) throw new ArgumentNullException(nameof(stadium), "El estadio no puede ser vacio");
    }

    public void SetMatchResult(int homeGoals, int awayGoals)
    {
        if (_clock.Now < _dateMatch)
            throw new InvalidOperationException("El partido todavía no se jugó.");
        if (_homeGoals != null || _awayGoals != null)
            throw new InvalidOperationException("El resultado ya fue seteado.");
        if (homeGoals < 0 || awayGoals < 0)
            throw new ArgumentException("Los goles no pueden ser negativos.");
        _homeGoals = homeGoals;
        _awayGoals = awayGoals;
        AssignWinner(homeGoals, awayGoals);
    }

    private void ValidateDate(DateTime dateMatch, IDateTimeMatch clock)
    {
        if (dateMatch < clock.Now)
            throw new ArgumentException("La fecha no puede ser pasada.");
    }
    
    private void ValidateClock(IDateTimeMatch clock)
    {
        if (clock == null)
            throw new ArgumentNullException(nameof(clock), "El reloj no puede ser vacío.");
    }
    
    private void AssignWinner(int homeGoals, int awayGoals)
    {
        if (homeGoals > awayGoals)
            _winnerMatch = _homeTeam;
        else if (homeGoals < awayGoals)
            _winnerMatch = _awayTeam;
        else
            _winnerMatch = null;
    }

    public void UpdateDate(DateTime newDate)
    {
        ValidateDate(newDate, _clock);
        _dateMatch = newDate;
    }
    
    public void UpdateStadium(IStadium newStadium)
    {
        ValidateStadium(newStadium);
        _stadium = newStadium;
    }
}