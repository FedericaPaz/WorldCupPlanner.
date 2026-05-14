using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class ConfederationDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "La cantidad máxima de equipos es obligatoria.")]
    public int MaxTeams { get; set; }
}
