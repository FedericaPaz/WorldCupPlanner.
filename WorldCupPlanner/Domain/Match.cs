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

    public int IdMatch
    {
        get => _idMatch;
    }

    public DateTime DateMatch
    {
        get => _dateMatch;
    }

    public IStadium Stadium
    {
        get => _stadium;
    }

    public ITeam HomeTeam
    {
        get => _homeTeam;
    }

    public ITeam AwayTeam
    {
        get => _awayTeam;
    }

    public char Group
    {
        get => _group;
    }

    public int? HomeGoals
    {
        get => _homeGoals;
    }

    public int? AwayGoals
    {
        get => _awayGoals;
    }

    public ITeam? WinnerMatch
    {
        get => _winnerMatch;
    }

    public Match(DateTime dateMatch, IStadium stadium, ITeam homeTeam, ITeam awayTeam, char group)
    {
        _idMatch = ++_idCounter;
        _dateMatch = dateMatch;
        _stadium = stadium;
        _homeTeam = homeTeam;
        _awayTeam = awayTeam;
        _group = group;
    }
}