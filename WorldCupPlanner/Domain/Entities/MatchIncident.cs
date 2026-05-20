using Domain.Enums;

namespace Domain.Entities;

public class MatchIncident
{
    private IncidentType _type;
    private Team _team;

    public IncidentType Type
    {
        get { return _type; }
    }

    public Team Team
    {
        get { return _team; }
    }
    
    public MatchIncident(IncidentType type, Team team, int quantity)
    {
        _type = type;
        _team = team;
    }
}