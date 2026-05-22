using DTOs.User;

namespace DTOs.Auth;

public class LoginResponse
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public List<UserRoleDto> Roles { get; set; } = new();
}