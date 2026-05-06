namespace Domain;

public class Standing
{
    private Team _team;
    private int _points;
    private int _goalsFor;
    private int _goalsAgainst;
    private int _goalDifference;
    private int _matchesPlayed;

    public Team TeamInGroup
    {
        get { return _team; }
    }

    public int PointsInGroup
    {
        get { return _points; }
        set { _points = value; }
    }

    public int GoalsFor
    {
        get { return _goalsFor; }
        set { _goalsFor = value; }
    }

    public int GoalsAgainst
    {
        get { return _goalsAgainst; }
        set { _goalsAgainst = value; }
    }

    public int GoalDifference
    {
        get { return GoalsFor - GoalsAgainst; }
    }

    public int MatchesPlayed
    {
        get { return _matchesPlayed; }
        set { _matchesPlayed = value; }
    }
    
    public Standing(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
        _team = team;
    }

    public void AddMatchResult(int goalsFor, int goalsAgainst)
    {
        MatchesPlayed++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        if (goalsFor > goalsAgainst)
            PointsInGroup += 3;
        else if (goalsFor ==  goalsAgainst)
            PointsInGroup += 1;
        if (goalsFor < 0 || goalsAgainst < 0)
            throw new ArgumentException("Los goles no pueden ser negativos.");
    }
}