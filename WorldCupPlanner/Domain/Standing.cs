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
    }

    public int GoalsFor
    {
        get { return _goalsFor; }
    }

    public int GoalsAgainst
    {
        get { return _goalsAgainst; }
    }
    
    public Standing(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
        _team = team;
    }
}