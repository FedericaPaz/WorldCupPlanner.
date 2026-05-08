namespace Domain;

public class ClockStub : IDateTimeMatch
{
    private readonly DateTime _now;
    public ClockStub(DateTime now)
    {
        _now = now;
    }
    public DateTime Now => _now;
}

public enum TournamentPhase
{
    NotStarted,
    GroupStage,
    RoundOf32,
    RoundOf16,
    QuarterFinals,
    SemiFinals,
    ThirdPlace,
    Final,
    Finished
}

public class Fixture
{
    public TournamentPhase CurrentPhase { get; private set; }
    private List<Group> _groups;
    private List<Match> _eliminatoryMatches; //eliminatoria
    private readonly TournamentConfig _config;
    
    public IReadOnlyList<Group> GetGroups
    {
        get => _groups;
    }

    public Fixture(TournamentConfig config)
    {
        _config = config;
        _groups = new List<Group>();
        CurrentPhase = TournamentPhase.NotStarted;
    }

    public void InitializeGroups()
    {
        _groups = new List<Group>();
        for (char c = 'A'; c <= 'L'; c++)
            _groups.Add(new Group(c));
    }
    
    public void AdvanceToPhase(TournamentPhase newPhase)
    {
        if (!CanAdvanceToPhase(newPhase))
            throw new InvalidOperationException($"No se puede avanzar a {newPhase}");
            
        CurrentPhase = newPhase;
    }
    
    public bool IsValid()
    {
        if (_groups.Count != 12)
            return false;
        
        foreach (var group in _groups)
        {
            if (group.TeamsInGroup.Count != 4)
                return false;
        
            if (group.MatchesInGroup.Count != 6)
                return false;
        }
        return true;
    }

    public bool CanAdvanceToPhase(TournamentPhase newPhase)
    {
        if (newPhase == TournamentPhase.GroupStage)
            return _groups.Count > 0;
    
        if (newPhase == TournamentPhase.RoundOf32)
            return CurrentPhase == TournamentPhase.GroupStage && AllGroupMatchesPlayed();
    
        if (newPhase == TournamentPhase.QuarterFinals)
            return CurrentPhase == TournamentPhase.RoundOf32 && AllKnockoutMatchesPlayed();
    
        return false;
    }

    private bool AllGroupMatchesPlayed()
    {
        return _groups.SelectMany(group => group.MatchesInGroup).All(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue);
    }

    private bool AllKnockoutMatchesPlayed()
    {
        return _eliminatoryMatches.All(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue);
    }
}