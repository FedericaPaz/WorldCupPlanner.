using DTOs.User;

namespace DTOs;

public class RegisterRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Password { get; set; } = "";
    public List<UserRoleDto> Roles { get; set; } = new();
}