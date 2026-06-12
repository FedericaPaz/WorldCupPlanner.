using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class GroupRepository : IGroupRepository
{
    private readonly WorldCupPlannerContext _context;

    public GroupRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddGroup(Group group)
    {
        if (group is null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        _context.Groups.Add(group);
        _context.SaveChanges();
    }

    public Group GetGroupByName(char groupName)
    {
        var group = _context.Groups
            .FirstOrDefault(group => group.GroupLetter == groupName);

        if (group is null)
        {
            throw new KeyNotFoundException();
        }

        return group;
    }

    public List<Group> GetAllGroups()
    {
        return _context.Groups
            .AsNoTracking()
            .ToList();
    }
    
    private void EnsureValidLetter(char groupLetter)
    {
        if (groupLetter < 'A' || groupLetter > 'L')
            throw new ArgumentException("El grupo debe ser una letra de la A a la L.");
    }
}