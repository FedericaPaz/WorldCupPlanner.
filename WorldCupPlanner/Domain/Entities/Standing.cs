namespace Domain.Entities;

public class Standing
{
    private const int PointsForWin = 3;
    private const int PointsForDraw = 1;

    private Team _team;
    private int _points;
    private int _goalsFor;
    private int _goalsAgainst;
    private int _matchesPlayed;
    private int _wins;
    private int _draws;
    private int _losses;

    public Team TeamInGroup
    {
        get { return _team; }
    }

    public int PointsInGroup
    {
        get { return _points; }
        private set { _points = value; }
    }

    public int GoalsFor
    {
        get { return _goalsFor; }
        private set { _goalsFor = value; }
    }

    public int GoalsAgainst
    {
        get { return _goalsAgainst; }
        private set { _goalsAgainst = value; }
    }

    public int GoalDifference
    {
        get { return GoalsFor - GoalsAgainst; }
    }

    public int MatchesPlayed
    {
        get { return _matchesPlayed; }
        private set { _matchesPlayed = value; }
    }

    public int Wins
    {
        get { return _wins; }
        private set { _wins = value; }
    }

    public int Draws
    {
        get { return _draws; }
        private set { _draws = value; }
    }

    public int Losses
    {
        get { return _losses; }
        private set { _losses = value; }
    }

    public Standing(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));

        _team = team;
    }

    public void AddMatchResult(int goalsFor, int goalsAgainst)
    {
        EnsureGoalsAreValid(goalsFor, goalsAgainst);

        UpdateGoals(goalsFor, goalsAgainst);

        MatchesPlayed++;

        UpdateResultStatistics(goalsFor, goalsAgainst);
    }

    private void EnsureGoalsAreValid(int goalsFor, int goalsAgainst)
    {
        if (goalsFor < 0)
            throw new ArgumentOutOfRangeException(nameof(goalsFor), "Los goles no pueden ser negativos.");

        if (goalsAgainst < 0)
            throw new ArgumentOutOfRangeException(nameof(goalsAgainst), "Los goles no pueden ser negativos.");
    }

    private void UpdateGoals(int goalsFor, int goalsAgainst)
    {
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
    }

    private void UpdateResultStatistics(int goalsFor, int goalsAgainst)
    {
        if (goalsFor > goalsAgainst)
            RegisterWin();
        else if (goalsFor == goalsAgainst)
            RegisterDraw();
        else
            RegisterLoss();
    }

    private void RegisterWin()
    {
        PointsInGroup += PointsForWin;
        Wins++;
    }

    private void RegisterDraw()
    {
        PointsInGroup += PointsForDraw;
        Draws++;
    }

    private void RegisterLoss()
    {
        Losses++;
    }
    
    public string GetTeamName() => _team.Name;
}