using System.ComponentModel.DataAnnotations;
using DTOs.User;

namespace DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string FirstName { get; set; } = "";
    
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string LastName { get; set; } = "";
    
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = "";
    
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime BirthDate { get; set; }
   
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = "";
    
    public List<UserRoleDto> Roles { get; set; } = new();
}