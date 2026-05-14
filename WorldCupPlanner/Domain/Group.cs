namespace Domain;

public class Group
{
    private const int MaxTeams = 4;
    private const int MaxMatches = 6;
    private const char FirstGroupLetter = 'A';
    private const char LastGroupLetter = 'L';
    
    private char _groupLetter;
    private readonly List<Team> _teamsInGroup = new List<Team>();
    private readonly List<Match> _matchesInGroup = new List<Match>();
    
    public char GroupLetter
    {
        get => _groupLetter;
    }

    public IReadOnlyList<Team> TeamsInGroup
    {
        get => _teamsInGroup;
    }

    public IReadOnlyList<Match> MatchesInGroup
    {
        get => _matchesInGroup;
    }
    
    public Group(char groupLetter)
    {
        EnsureValidGroupLetter(groupLetter);
        _groupLetter = groupLetter;
    }

    private static void EnsureValidGroupLetter(char groupLetter)
    {
        if (groupLetter < FirstGroupLetter || groupLetter > LastGroupLetter) 
            throw new ArgumentException("El grupo debe ser una letra de la A a la L.");
    }
    
    public void AddTeamToGroup(Team team)
    {
        EnsureTeamIsNotNull(team);
        EnsureGroupHasSpaceForTeam();
        EnsureTeamIsNotAlreadyInGroup(team);
        _teamsInGroup.Add(team);
    }

    private static void EnsureTeamIsNotNull(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
    }

    private void EnsureGroupHasSpaceForTeam()
    {
        if (_teamsInGroup.Count >= MaxTeams)
            throw new InvalidOperationException("El grupo ya tiene 4 equipos.");
    }

    private void EnsureTeamIsNotAlreadyInGroup(Team team)
    {
        if (_teamsInGroup.Contains(team))
            throw new InvalidOperationException("El equipo ya está en el grupo.");
    }
    
    
    public void AddMatchToGroup(Match match)
    {
        EnsureMatchIsNotNull(match);
        EnsureMatchBelongsToGroup(match);
        EnsureGroupHasSpaceForMatch();
        EnsureMatchTeamsBelongToGroup(match);
        EnsureMatchIsNotAlreadyRegistered(match);
        _matchesInGroup.Add(match);
    }
    

    private static void EnsureMatchIsNotNull(Match match)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));
    }

    private void EnsureMatchBelongsToGroup(Match match)
    {
        if (match.Group != GroupLetter)
            throw new ArgumentException("El partido no pertenece a este grupo.");
    }

    private void EnsureGroupHasSpaceForMatch()
    {
        if (_matchesInGroup.Count >= 6)
            throw new InvalidOperationException("El grupo ya tiene 6 partidos.");
    }
    
    private void EnsureMatchTeamsBelongToGroup(Match match)
    {
        if (!_teamsInGroup.Contains(match.HomeTeam) || !_teamsInGroup.Contains(match.AwayTeam))
            throw new InvalidOperationException("Los equipos del partido deben pertenecer al grupo.");
    }

    private void EnsureMatchIsNotAlreadyRegistered(Match match)
    {
        if (_matchesInGroup.Contains(match))
            throw new InvalidOperationException("El partido ya está registrado en el grupo.");
    }

    public void RemoveTeamFromGroup(Team team)
    {
        _teamsInGroup.Remove(team);
    }
}