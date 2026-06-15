using DTOs.User;

namespace IServices.UserInterfaces;

public interface IUserQueryService
{
    List<UserResponseDto> GetAllUsers();
    UserResponseDto? GetUserDto(string email);
    bool CanExport(string email);
}