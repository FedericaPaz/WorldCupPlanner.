using DTOs;

namespace IServices;

public interface IAuthService
{
    LoginResponse Login(LoginRequest request);
    void Register(RegisterRequest request);
}