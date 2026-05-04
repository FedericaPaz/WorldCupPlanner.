namespace Domain;

public class Group
{
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
        if (groupLetter < 'A' || groupLetter > 'L') 
            throw new ArgumentException("El grupo debe ser una letra de la A a la L.");
        _groupLetter = groupLetter;
    }
    
    public void AddTeamToGroup(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
        if (_teamsInGroup.Count >= 4)
            throw new InvalidOperationException("El grupo ya tiene 4 equipos.");
        if (_teamsInGroup.Contains(team))
            throw new InvalidOperationException("El equipo ya está en el grupo.");
        _teamsInGroup.Add(team);
    }
    
    public void AddMatchToGroup(Match match)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));
        if (match.Group != GroupLetter)
            throw new ArgumentException("El partido no pertenece a este grupo.");
        if (_matchesInGroup.Count >= 6)
            throw new InvalidOperationException("El grupo ya tiene 6 partidos.");
        if (!_teamsInGroup.Contains(match.HomeTeam))
            throw new InvalidOperationException("El equipo del partido debe pertenecer al grupo.");
        _matchesInGroup.Add(match);
    }
}