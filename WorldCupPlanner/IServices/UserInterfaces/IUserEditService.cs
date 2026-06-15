using Domain.Enums;

namespace IServices.UserInterfaces;

public interface IUserEditService
{
    void DeleteUser(string email, string currentUserEmail);
    void UpdateUserWithoutPassword(string firstName, string lastName, string email, DateTime birthDate, List<UserRole> roles);
    string ResetPassword(string email);
    void ChangeOwnPassword(string email, string newPassword);
}