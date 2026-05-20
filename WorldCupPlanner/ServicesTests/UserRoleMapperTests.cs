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
}