using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class MatchRequest
{
    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime dateMatch { get; set; }

    [Required(ErrorMessage = "El estadio es obligatorio")]
    public string stadiumName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El equipo local es obligatorio")]
    public string homeTeamName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El equipo visitante es obligatorio")]
    public string awayTeamName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El grupo es obligatorio")]
    [RegularExpression("^[A-La-l]$", ErrorMessage = "El grupo debe ser una letra entre A y L")]
    public string group { get; set; } = string.Empty;
}