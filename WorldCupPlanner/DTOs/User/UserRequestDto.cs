using System.ComponentModel.DataAnnotations;
using Domain;

namespace DTOs.User;

public class UserRequestDto
{
    [Required(ErrorMessage = "El nombre no puede estar vacío.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido no puede estar vacío.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email no puede estar vacío.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "La contraseña no puede estar vacía.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
        ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial."
    )]
    public string Password { get; set; } = string.Empty;
    public List<UserRoleDto> Roles { get; set; } = new();
}