using Domain.Enums;

namespace Domain.Entities;

public class MatchIncident
{
    private IncidentType _type;
    private Team _team;
    private int _quantity;

    public IncidentType Type => _type;
    public Team Team => _team;
    public int Quantity => _quantity;
    
    public MatchIncident(IncidentType type, Team team, int quantity)
    {
        EnsureTeamIsNotNull(team);
        EnsureQuantityIsValid(quantity);
        
        _type = type;
        _team = team;
        _quantity = quantity;
    }

    private static void EnsureTeamIsNotNull(Team team)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));
    }

    private static void EnsureQuantityIsValid(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("La cantidad de incidencias no puede ser negativa.");
    }
}