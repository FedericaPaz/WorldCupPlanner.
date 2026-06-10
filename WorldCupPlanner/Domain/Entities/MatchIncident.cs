using Domain.Enums;

namespace Domain.Entities;

public class MatchIncident
{
    private int _id;
    private IncidentType _type;
    private Team _team;
    private string _teamName = string.Empty;
    private int _quantity;
    private int _matchId;
    private Match _match = null!;
    
    public int Id => _id;
    public IncidentType Type => _type;
    public Team Team => _team;
    public string TeamName => _teamName;
    public int Quantity => _quantity;
    public int MatchId => _matchId;
    public Match Match => _match;
    
    private MatchIncident()
    {
    }
    
    public MatchIncident(IncidentType type, Team team, int quantity, int matchId = 0)
    {
        EnsureTeamIsNotNull(team);
        EnsureQuantityIsValid(quantity);

        _type = type;
        _team = team;
        _teamName = team.Name;
        _quantity = quantity;
        _matchId = matchId;
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