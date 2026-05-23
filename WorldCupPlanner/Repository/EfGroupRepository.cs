using Domain.Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EfGroupRepository : IGroupRepository
{
    private readonly WorldCupPlannerContext _context;

    public EfGroupRepository(WorldCupPlannerContext context)
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
}