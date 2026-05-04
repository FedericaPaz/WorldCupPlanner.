namespace DTOs.User;
using Domain;

public class UserResponseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public List<UserRole> Roles { get; set; } = new();
}