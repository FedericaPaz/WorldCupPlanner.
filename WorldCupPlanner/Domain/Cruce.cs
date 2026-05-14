namespace Domain;

public class Cruce
{
    private string _nombre;
    private Match _partido;
    private TournamentPhase _phase;
    private Cruce? _nextCruce;

    public string Nombre
    {
        get { return _nombre; }
        private set { _nombre = value; }
    }

    public Match Partido
    {
        get { return _partido; }
        private set { _partido = value; }
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
    
    public Cruce(string nombre, Match partido, TournamentPhase phase)
    {
        if (string.IsNullOrEmpty(nombre))
            throw new ArgumentException("El nombre del cruce no puede ser vacío o nulo", nameof(nombre));
        
        if (partido == null)
            throw new ArgumentNullException(nameof(partido), "El partido no puede ser nulo");
        
        Nombre = nombre;
        Partido = partido;
        Phase = phase;
    }

    public void DecideNextCruce(Cruce cruce)
    {
        if (cruce == null)
            throw new ArgumentNullException(nameof(cruce), "El cruce no puede ser nulo");
        if (cruce == this)
            throw new InvalidOperationException("El cruce no puede ser igual a sí mismo");
        if (!IsValidNextPhase(cruce.Phase))
            throw new InvalidOperationException($"El cruce {cruce.Nombre} no es válido para la fase {Phase}");
        NextCruce = cruce;
    }

    public ITeam? GetWinner()
    {
        return Partido.WinnerMatch;
    }
    
    public ITeam? GetLoser()
    {
        return Partido.LoserMatch;
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