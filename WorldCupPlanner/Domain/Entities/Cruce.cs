using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Cruce
{
    private string _name;
    private Match _match;
    private TournamentPhase _phase;
    private Cruce? _nextCruce;

    public int Id { get; private set; }
    public int MatchId { get; private set; }
    public Match Match { get; private set; } = null!;

    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    public TournamentPhase Phase
    {
        get { return _phase; }
        private set { _phase = value; }
    }

    public Cruce? NextCruce
    {
        get { return _nextCruce; }
        private set { _nextCruce = value; }
    }

    private Cruce()
    {
        _name = string.Empty;
        _match = null!;
    }

    public Cruce(string name, Match match, TournamentPhase phase)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del cruce no puede ser vacío o nulo", nameof(name));

        Name = name;
        Match = match;
        MatchId = match.IdMatch;
        Phase = phase;
    }

    public void DecideNextCruce(Cruce cruce)
    {
        if (cruce == null)
            throw new ArgumentNullException(nameof(cruce), "El cruce no puede ser nulo");
        if (cruce == this)
            throw new InvalidOperationException("El cruce no puede ser igual a sí mismo");
        if (!IsValidNextPhase(cruce.Phase))
            throw new InvalidOperationException($"El cruce {cruce.Name} no es válido para la fase {Phase}");
        NextCruce = cruce;
    }

    public ITeam? GetWinner()
    {
        return Match.WinnerMatch;
    }

    public ITeam? GetLoser()
    {
        return Match.LoserMatch;
    }

    private bool IsValidNextPhase(TournamentPhase nextPhase)
    {
        return nextPhase == GetNextPhase();
    }

    private static readonly Domain.Interfaces.IPhaseTransitionPolicy _defaultPhasePolicy = new Domain.Policies.DefaultPhaseTransitionPolicy();

    public TournamentPhase GetNextPhase()
    {
        return _defaultPhasePolicy.GetNextPhase(Phase);
    }
}