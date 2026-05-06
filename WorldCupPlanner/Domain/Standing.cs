namespace Domain;

public class Standing
{
    private Team _team;
    private int _points;
    private int _goalsFor;
    private int _goalsAgainst;

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
    
    public Standing(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
        _team = team;
    }

    public void AddMatchResult(int goalsFor, int goalsAgainst)
    {
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        if (goalsFor > goalsAgainst)
            PointsInGroup += 3;
        else if (goalsFor ==  goalsAgainst)
            PointsInGroup += 1;
    }
}