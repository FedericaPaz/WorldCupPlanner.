using Domain.Entities;
using Domain.Enums;
using DTOs.Match;

namespace Services.Mappers;

public static class MatchResultMapper
{
    public static IEnumerable<MatchIncident> ToIncidents(this MatchResultRequest request, Match match)
    {
        var incidents = new List<MatchIncident>();

        if (request == null)
            return incidents;

        if (request.homeYellowCards > 0)
            incidents.Add(new MatchIncident(IncidentType.YellowCard, match.HomeTeam, request.homeYellowCards));

        if (request.awayYellowCards > 0)
            incidents.Add(new MatchIncident(IncidentType.YellowCard, match.AwayTeam, request.awayYellowCards));

        if (request.homeRedCards > 0)
            incidents.Add(new MatchIncident(IncidentType.RedCard, match.HomeTeam, request.homeRedCards));

        if (request.awayRedCards > 0)
            incidents.Add(new MatchIncident(IncidentType.RedCard, match.AwayTeam, request.awayRedCards));

        return incidents;
    }
}
