using Domain;
using IRepository;

namespace Repository;

public class GroupRepository : IGroupRepository
{
    private readonly List<Group> _group = new();
    public void AddGroup(Group group)
    {
        _group.Add(group);
    }

    public Group GetGroupByName(string groupName)
    {
        throw new NotImplementedException();
    }

    public List<Group> GetAllGroups()
    {
        return _group.ToList();
    }
}