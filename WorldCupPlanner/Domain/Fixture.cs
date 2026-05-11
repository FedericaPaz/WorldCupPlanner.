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

public class Fixture
{
    public TournamentPhase CurrentPhase { get; private set; }
    private List<Group> _groups;
    private List<Match> _eliminatoryMatches;
    private readonly TournamentConfig _config;
    
    public List<Group> GetGroups
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
        return newPhase switch
        {
            TournamentPhase.GroupStage => _groups.Count > 0,
            TournamentPhase.RoundOf32 => CurrentPhase == TournamentPhase.GroupStage && AllGroupMatchesPlayed(),
            TournamentPhase.RoundOf16 => CurrentPhase == TournamentPhase.RoundOf32 && AllKnockoutMatchesPlayed(),
            TournamentPhase.QuarterFinals => CurrentPhase == TournamentPhase.RoundOf16 && AllKnockoutMatchesPlayed(),
            TournamentPhase.SemiFinals => CurrentPhase == TournamentPhase.QuarterFinals && AllKnockoutMatchesPlayed(),
            TournamentPhase.ThirdPlace or TournamentPhase.Final => CurrentPhase == TournamentPhase.SemiFinals && AllKnockoutMatchesPlayed(),
            TournamentPhase.Finished => CurrentPhase == TournamentPhase.Final && AllKnockoutMatchesPlayed(),
            _ => false
        };
    }

    private bool AllGroupMatchesPlayed()
    {
        return _groups.SelectMany(group => group.MatchesInGroup).All(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue);
    }

    private bool AllKnockoutMatchesPlayed()
    {
        return _eliminatoryMatches.All(match => match.HomeGoals.HasValue && match.AwayGoals.HasValue);
    }
    
    public bool IsStillValid(List<Team> existingTeams, List<Stadium> existingStadiums)
    {
        if (!IsValid()) return false;
        foreach (var group in _groups)
        {
            foreach (var team in group.TeamsInGroup)
                if (existingTeams.All(t => t.Name != team.Name)) return false;
        }
        foreach (var group in _groups)
        {
            foreach (var match in group.MatchesInGroup)
                if (existingStadiums.All(s => s.StadiumName != match.Stadium.StadiumName)) return false;
        }
        return true;
    }
}