using Domain.Enums;

namespace Domain.Entities;

public class MatchIncident
{
    private IncidentType _type;

    public IncidentType Type
    {
        get { return _type; }
    }
    
    public MatchIncident(IncidentType type, Team team, int quantity)
    {
        _type = type;
    }
}