using System.Runtime.InteropServices.JavaScript;

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
        for (char c = 'A'; c <= 'L'; c++)
            _groups.Add(new Group(c));
    }
}