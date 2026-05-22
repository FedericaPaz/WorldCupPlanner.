using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public class UserEditRequestDto
{
    [Required(ErrorMessage = "El nombre no puede estar vacío.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido no puede estar vacío.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime BirthDate { get; set; }

    public List<UserRoleDto> Roles { get; set; } = new();
}