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

    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    public Match Match
    {
        get { return _match; }
        private set { _match = value; }
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
    }
    
    public Cruce(string name, Match match, TournamentPhase phase)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("El nombre del cruce no puede ser vacío o nulo", nameof(name));
        
        if (match == null)
            throw new ArgumentNullException(nameof(match), "El partido no puede ser nulo");
        
        Name = name;
        Match = match;
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

    public TournamentPhase GetNextPhase()
    {
        return Phase switch
        {
            TournamentPhase.RoundOf32 => TournamentPhase.RoundOf16,
            TournamentPhase.RoundOf16 => TournamentPhase.QuarterFinals,
            TournamentPhase.QuarterFinals => TournamentPhase.SemiFinals,
            TournamentPhase.SemiFinals => TournamentPhase.Final,
            TournamentPhase.Final => throw new InvalidOperationException("La final no tiene siguiente fase"),
            TournamentPhase.ThirdPlace => throw new InvalidOperationException("El partido por el tercer puesto no tiene siguiente fase"),
            _ => throw new InvalidOperationException($"La fase {Phase} no tiene siguiente definida")
        };
    }
}