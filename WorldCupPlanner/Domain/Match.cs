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
        ValidateGroup(group);
        ValidateTeams(homeTeam, awayTeam);
        ValidateStadium(stadium);

        _idMatch = ++_idCounter;
        _dateMatch = dateMatch;
        _stadium = stadium;
        _homeTeam = homeTeam;
        _awayTeam = awayTeam;
        _group = char.ToUpper(group); 
        _clock = clock;
    }
    
    private void ValidateGroup(char group)
    {
        if (group == '\0')  throw new ArgumentNullException("Group must not be empty.");
        char groupToUpper = char.ToUpper(group);
        if (groupToUpper < 'A' || groupToUpper > 'L') throw new ArgumentException($"Group must be a letter between A and L. Value received: '{group}'.");
    }
    
    private void ValidateTeams(ITeam homeTeam, ITeam awayTeam)
    {
        if (homeTeam == null) throw new ArgumentNullException(nameof(homeTeam), "Home team must not be null.");
        if (awayTeam == null) throw new ArgumentNullException(nameof(awayTeam), "Away team must not be null.");
        if (homeTeam == awayTeam) throw new ArgumentException("Home team and away team must be different.");
    }

    private void ValidateStadium(IStadium stadium)
    {
        if (stadium == null) throw new ArgumentNullException(nameof(stadium), "Stadium must not be null.");
    }

    public void SetMatchResult(int homeGoals, int awayGoals)
    {
        if (_homeGoals != null || _awayGoals != null)
            throw new InvalidOperationException("Result already set.");
        if (homeGoals < 0 || awayGoals < 0)
            throw new ArgumentException("Goals cannot be negative.");
        _homeGoals = homeGoals;
        _awayGoals = awayGoals;
        if (homeGoals > awayGoals)
            _winnerMatch = _homeTeam;
        else if (homeGoals < awayGoals)
            _winnerMatch = _awayTeam;
        else
            _winnerMatch = null;
    }


}