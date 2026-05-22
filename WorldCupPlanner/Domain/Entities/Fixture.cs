using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Fixture
{
    private TournamentPhase _currentPhase;
    private List<Group> _groups;
    private List<Match> _eliminatoryMatches;
    private readonly TournamentConfig _config;

    private const int ExpectedGroups = 12;
    private const int TeamsPerGroup = 4;
    private const int MatchesPerGroup = 6;

    public TournamentPhase CurrentPhase
    {
        get { return _currentPhase; }
        private set { _currentPhase = value; }
    }

    public IReadOnlyList<Group> Groups => _groups;

    public Fixture(TournamentConfig config)
    {
        _config = config;
        _groups = new List<Group>();
        _eliminatoryMatches = new List<Match>();
        CurrentPhase = TournamentPhase.NotStarted;
    }

    public void InitializeGroups()
    {
        _groups = new List<Group>();
        for (int i = 0; i < ExpectedGroups; i++)
            _groups.Add(new Group((char)('A' + i)));
    }

    public void AdvanceToPhase(TournamentPhase newPhase)
    {
        if (!CanAdvanceToPhase(newPhase))
            throw new InvalidOperationException($"No se puede avanzar a {newPhase}");

        CurrentPhase = newPhase;
    }

    public bool IsValid()
    {
        if (_groups.Count != ExpectedGroups)
            return false;

        foreach (var group in _groups)
        {
            if (group.TeamsInGroup.Count != TeamsPerGroup)
                return false;

            if (group.MatchesInGroup.Count != MatchesPerGroup)
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

    public bool IsStillValid(List<ITeam> existingTeams, List<IStadium> existingStadiums)
    {
        if (!IsValid()) return false;
        if (existingTeams == null) throw new ArgumentNullException(nameof(existingTeams));
        if (existingStadiums == null) throw new ArgumentNullException(nameof(existingStadiums));
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

    public void AddEliminatoryMatch(Match match)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));
        _eliminatoryMatches.Add(match);
    }
    public void SetGroups(List<Group> groups)
    {
        if (groups == null) throw new ArgumentNullException(nameof(groups));
        _groups = groups;
    }
}