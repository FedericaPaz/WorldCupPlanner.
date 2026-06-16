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

    private static readonly Dictionary<string, Func<ISimulationEngine>> _engines = new();

    public TournamentPhase CurrentPhase
    {
        get { return _currentPhase; }
        private set { _currentPhase = value; }
    }
    public string SimulationEngineName { get; private set; } = string.Empty;
    public IReadOnlyList<Group> Groups => _groups;
    public int Id { get; private set; }

    private Fixture()
    {
        _groups = new List<Group>();
        _eliminatoryMatches = new List<Match>();
    }

    public Fixture(TournamentConfig config)
    {
        _config = config;
        _groups = new List<Group>();
        _eliminatoryMatches = new List<Match>();
        CurrentPhase = TournamentPhase.NotStarted;
    }

    public static void RegisterEngine(string name, Func<ISimulationEngine> factory)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del motor no puede ser vacío.", nameof(name));
        _engines[name] = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public ISimulationEngine CreateEngine()
    {
        if (_engines.TryGetValue(SimulationEngineName, out var factory))
            return factory();

        throw new InvalidOperationException($"No hay motor registrado con el nombre '{SimulationEngineName}'.");
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
            if (!group.HasExpectedTeams())
                return false;
            if (!group.HasExpectedMatches())
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

    public bool IsStillValid(List<ITeam> existingTeams, List<IStadium> existingStadiums)
    {
        return _groups.Count == ExpectedGroups && existingStadiums.Count >= 4 && existingTeams.Count == 48;
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

    public void SetSimulationEngine(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del motor no puede ser vacío.");
        SimulationEngineName = name;
    }

    public void CargarPartidos(IEnumerable<Match> partidos)
    {
        foreach (var group in _groups)
            foreach (var match in partidos.Where(m => m.Group == group.GroupLetter))
                group.LoadMatchFromRepository(match);
    }

    public void ResetRankings()
    {
        foreach (var group in _groups)
            foreach (var team in group.TeamsInGroup)
                team.ResetRankingToFifa();
    }

    public void LockGroupMatches()
    {
        foreach (var group in _groups)
            foreach (var match in group.MatchesInGroup)
                match.Lock();
    }

    public Dictionary<string, char> GetTeamToGroupMap()
    {
        return _groups
            .SelectMany(g => g.GetTeamNames().Select(name => (name, g.GroupLetter)))
            .ToDictionary(x => x.name, x => x.GroupLetter);
    }

    public List<Team> GetPrimeros()
        => SortAndBreakTies(_groups
            .Select(g => g.GetStandings().FirstOrDefault())
            .Where(s => s != null)
            .Cast<Standing>()
            .ToList());

    public List<Team> GetSegundos()
        => SortAndBreakTies(_groups
            .Select(g => g.GetStandings().ElementAtOrDefault(1))
            .Where(s => s != null)
            .Cast<Standing>()
            .ToList());

    public List<Team> GetTerceros()
        => SortAndBreakTies(_groups
            .Select(g => g.GetStandings().ElementAtOrDefault(2))
            .Where(s => s != null)
            .Cast<Standing>()
            .ToList());
    
    private List<Team> SortAndBreakTies(List<Standing> standings)
    {
        var ordenados = standings
            .OrderByDescending(s => s.PointsInGroup)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();

        var resultado = new List<Standing>();
        var rng = new Random(_config.SemillaCrucesFase);
        int i = 0;

        while (i < ordenados.Count)
        {
            int j = i + 1;
            while (j < ordenados.Count
                   && ordenados[j].PointsInGroup == ordenados[i].PointsInGroup
                   && ordenados[j].GoalDifference == ordenados[i].GoalDifference
                   && ordenados[j].GoalsFor == ordenados[i].GoalsFor)
                j++;

            var empatados = ordenados.Skip(i).Take(j - i).ToList();
            if (empatados.Count > 1) empatados = ApplyFisherYates(empatados, rng);

            resultado.AddRange(empatados);
            i = j;
        }
        return resultado.Select(s => s.TeamInGroup).ToList();
    }

    private static List<T> ApplyFisherYates<T>(List<T> list, Random rng)
    {
        var arr = list.ToArray();
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr.ToList();
    }

    private bool AllGroupMatchesPlayed()
    {
        return _groups.SelectMany(g => g.MatchesInGroup)
                      .All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue);
    }

    private bool AllKnockoutMatchesPlayed()
    {
        return _eliminatoryMatches.All(m => m.HomeGoals.HasValue && m.AwayGoals.HasValue);
    }
}