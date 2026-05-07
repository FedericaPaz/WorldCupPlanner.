using Domain;

namespace IRepository;

public interface IGroupRepository
{
    void AddGroup(Group group);
    Group GetGroupByName(string groupName);
    List<Group> GetAllGroups();
}