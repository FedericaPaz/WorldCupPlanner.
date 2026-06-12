using System.ComponentModel.DataAnnotations;

namespace DTOs.Team;

public class TeamDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 60 caracteres.")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "La confederación es obligatoria.")]
    public string ConfederationName { get; set; } = null!;
   
    [Required(ErrorMessage = "El ranking FIFA es obligatorio.")]
    [Range(300, 2500, ErrorMessage = "El ranking FIFA debe estar entre 300 y 2500.")]
    public int FifaRanking { get; set; }
    public int CurrentRanking { get; set; }
    public string? FlagBase64 { get; set; }
}