using System.ComponentModel.DataAnnotations;

namespace DTOs.Match;

public class MatchResultRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "Los goles del equipo local no pueden ser negativos")]
    public int homeGoals { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Los goles del equipo visitante no pueden ser negativos")]
    public int awayGoals { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Las tarjetas amarillas del equipo local no pueden ser negativas")]
    public int homeYellowCards { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Las tarjetas amarillas del equipo visitante no pueden ser negativas")]
    public int awayYellowCards { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Las tarjetas rojas del equipo local no pueden ser negativas")]
    public int homeRedCards { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Las tarjetas rojas del equipo visitante no pueden ser negativas")]
    public int awayRedCards { get; set; }
}