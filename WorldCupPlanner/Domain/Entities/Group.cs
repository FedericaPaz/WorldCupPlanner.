namespace Domain.Entities;

public class Group
{
    private const int MaxTeams = 4;
    private const int MaxMatches = 6;
    private const char FirstGroupLetter = 'A';
    private const char LastGroupLetter = 'L';

    private char _groupLetter;
    private readonly List<Team> _teamsInGroup = new();
    private readonly List<Match> _matchesInGroup = new();
    
    public int FixtureId { get; private set; }

    public int Id { get; private set; }
    
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
    
    private Group()
    {
        
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
            throw new InvalidOperationException($"El grupo ya tiene {MaxTeams} equipos.");
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
        if (_matchesInGroup.Count >= MaxMatches)
            throw new InvalidOperationException($"El grupo ya tiene {MaxMatches} partidos.");
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
        EnsureTeamIsNotNull(team);
        if (!_teamsInGroup.Contains(team))
            throw new InvalidOperationException("El equipo no pertenece al grupo.");
        _teamsInGroup.Remove(team);
    }
    
    public void LoadMatchFromRepository(Match match)
    {
        var existing = _matchesInGroup.FirstOrDefault(m => m.IdMatch == match.IdMatch);
        if (existing == null)
            _matchesInGroup.Add(match);
        else
        {
            var index = _matchesInGroup.IndexOf(existing);
            _matchesInGroup[index] = match;
        }
    }

    public List<Team> GetTeamsInGroup()
    {
        return _matchesInGroup
            .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
            .DistinctBy(t => t.Name)
            .Cast<Team>()
            .ToList();
    }
    
    public List<Standing> GetStandings()
    {
        var teams = _teamsInGroup.Any()
            ? _teamsInGroup.Where(t => t != null).ToList()
            : _matchesInGroup
                .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
                .Where(t => t != null)
                .DistinctBy(t => t.Name)
                .ToList();

        if (!teams.Any())
            return new List<Standing>();

        var standings = teams.ToDictionary(
            t => t.Name,
            t => new Standing(t));

        foreach (var match in _matchesInGroup.Where(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue))
        {
            if (match.HomeTeam == null || match.AwayTeam == null) continue;
            var homeName = match.GetHomeTeamName();
            var awayName = match.GetAwayTeamName();
            if (!standings.ContainsKey(homeName)) continue;
            if (!standings.ContainsKey(awayName)) continue;

            standings[homeName].AddMatchResult(match.HomeGoals!.Value, match.AwayGoals!.Value);
            standings[awayName].AddMatchResult(match.AwayGoals!.Value, match.HomeGoals!.Value);
        }

        return standings.Values
            .Where(s => s.TeamInGroup != null)
            .OrderByDescending(s => s.PointsInGroup)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();
    }

    public bool AllMatchesPlayed()
        => _matchesInGroup.Count == MaxMatches
           && _matchesInGroup.All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue);

    public IEnumerable<Match> GetPendingMatches()
        => _matchesInGroup.Where(m => !m.HomeGoals.HasValue && !m.AwayGoals.HasValue);
    
    private const int MaxUefaTeamsPerGroup = 2;
    private const string ConfederationUefa = "UEFA";

    public bool CanAddTeam(Team team)
    {
        if (team.GetConfederationName() == ConfederationUefa)
            return _teamsInGroup.Count(t => t.GetConfederationName() == ConfederationUefa) < MaxUefaTeamsPerGroup;

        return _teamsInGroup.All(t => t.GetConfederationName() != team.GetConfederationName());
    }

    public bool HasExpectedTeams() => _teamsInGroup.Count == MaxTeams;
    public bool HasExpectedMatches() => _matchesInGroup.Count == MaxMatches;
    public IEnumerable<string> GetTeamNames() => _teamsInGroup.Select(t => t.Name);
}