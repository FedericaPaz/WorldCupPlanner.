using Domain;
using IRepository;

namespace Repository;

public class GroupRepository : IGroupRepository
{
    private readonly List<Group> _group = new();
    public void AddGroup(Group group)
    {
        if(group is null)
            throw new ArgumentNullException(nameof(group));
        
        _group.Add(group);
    }

    public Group GetGroupByName(char groupName)
    {
        EnsureValidLetter(groupName);
        Group? group = _group.FirstOrDefault(g => g.GroupLetter == groupName);
        if (group is null)
            throw new KeyNotFoundException();
        
        return group;
    }

    public List<Group> GetAllGroups()
    {
        return _group.ToList();
    }

    private void EnsureValidLetter(char groupLetter)
    {
        if (groupLetter < 'A' || groupLetter > 'L')
            throw new ArgumentException("El grupo debe ser una letra de la A a la L.");
    }
}