using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class StadiumRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(80, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 80 caracteres")]
    public string stadiumName { get; set; } = string.Empty;   
    
    [Required(ErrorMessage = "La ciudad es obligatoria")]
    [StringLength(60, MinimumLength = 1, ErrorMessage = "La ciudad debe tener entre 1 y 60 caracteres")]
    public string stadiumCity { get; set; } = string.Empty;
    
    [StringLength(400, ErrorMessage = "La descripcion debe tener como maximo 400 caracteres")]
    public string? stadiumDescription { get; set; } = null;
    
    [Required(ErrorMessage = "La capacidad locativa es obligatoria")]
    [Range(20000, int.MaxValue, ErrorMessage = "La capacidad locativa debe ser al menos 20000")]
    public int locativeCapacity { get; set; }
}