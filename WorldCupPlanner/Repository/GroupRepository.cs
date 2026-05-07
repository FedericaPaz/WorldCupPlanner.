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

    public Group GetGroupByName(char groupName)
    {
        Group? group = _group.FirstOrDefault(g => g.GroupLetter == groupName);
        if (group is null)
            throw new KeyNotFoundException();
        
        return group;
    }

    public List<Group> GetAllGroups()
    {
        return _group.ToList();
    }
}