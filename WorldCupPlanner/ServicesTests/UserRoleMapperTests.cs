using DTOs.User;
using Domain.Enums;
using Services.Mappers;

namespace ServicesTests;

[TestClass]
public class UserRoleMapperTests
{
    [TestMethod]
    public void JournalistDomainRole_MappingToDto_ShouldReturnJournalistDto()
    {
        var result = UserRoleMapper.ToDto(UserRole.Journalist);
        Assert.AreEqual(UserRoleDto.Journalist, result);
    }
    
    [TestMethod]
    public void JournalistDtoRole_MappingToDomain_ShouldReturnJournalistDomainRole()
    {
        var result = UserRoleMapper.ToDomain(UserRoleDto.Journalist);
        Assert.AreEqual(UserRole.Journalist, result);
    }
}