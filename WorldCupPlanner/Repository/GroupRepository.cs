using Domain.Entities;
using IRepository;

namespace Repository;

public class GroupRepository : IGroupRepository
{
    private WorldCupPlannerContext _context = null!;

    public GroupRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }
    
    public void AddGroup(Group group)
    {
        if(group is null)
            throw new ArgumentNullException();
        
        _context.Add(group);
        _context.SaveChanges();
    }

    public Group GetGroupByName(char groupName)
    {
        EnsureValidLetter(groupName);
        Group? group = _context.Groups.FirstOrDefault(g => g.GroupLetter == groupName);
        if (group is null)
            throw new KeyNotFoundException();
        
        return group;
    }

    public List<Group> GetAllGroups()
    {
        return _context.Groups.ToList();
    }

    private void EnsureValidLetter(char groupLetter)
    {
        if (groupLetter < 'A' || groupLetter > 'L')
            throw new ArgumentException("El grupo debe ser una letra de la A a la L.");
    }
}