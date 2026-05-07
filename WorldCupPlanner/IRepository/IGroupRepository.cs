using Domain;

namespace IRepository;

public interface IGroupRepository
{
    void AddGroup(Group group);
    Group GetGroupByName(char groupName);
    List<Group> GetAllGroups();
}