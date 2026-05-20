using Domain.Enums;

namespace Domain.Entities;

public class MatchIncident
{
    private IncidentType _type;
    private Team _team;
    private int _quantity;

    public IncidentType Type
    {
        get { return _type; }
    }

    public Team Team
    {
        get { return _team; }
    }

    public int Quantity
    {
        get { return _quantity; }
    }
    
    public MatchIncident(IncidentType type, Team team, int quantity)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
        
        if (quantity < 0)
            throw new ArgumentException("La cantidad de incidencias no puede ser negativa.");
        
        _type = type;
        _team = team;
        _quantity = quantity;
    }
}