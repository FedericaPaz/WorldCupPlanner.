using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = "";
    
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = "";
}