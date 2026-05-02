using DTOs;
using Domain;

namespace Services.Mappers;

public class UserMapper
{
    public static User ToEntity(RegisterRequest dto)
    {
        return new User(dto.FirstName, dto.LastName, dto.Email, dto.BirthDate, dto.Password);
    }
}