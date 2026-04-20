using Domain;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private readonly List<Stadium> _stadiums = new List<Stadium>();

    public void AddStadium(Stadium? stadium)
    {
        if(stadium is null)
            throw new ArgumentNullException(nameof(stadium));
        _stadiums.Add(stadium);
    }

    public Stadium? GetStadium(Stadium? stadium)
    {
        if (stadium is null)
            throw new ArgumentNullException(nameof(stadium));
        return _stadiums.FirstOrDefault(s => s._name == stadium._name);
    }

    public void UpdateStadium(Stadium? stadium)
    {
        if (stadium is null)
            throw new ArgumentNullException(nameof(stadium));

        var existingStadium = _stadiums.FirstOrDefault(s => s._name == stadium._name);

        if (existingStadium is null)
            throw new InvalidOperationException($"Stadium {stadium._name} not found");

        _stadiums.Remove(existingStadium);
        _stadiums.Add(stadium);
    }

    public void DeleteStadium(Stadium stadium)
    {
        if (stadium is null)
            throw new ArgumentNullException(nameof(stadium));

        var existingStadium = _stadiums.FirstOrDefault(s => s._name == stadium._name);

        if (existingStadium is null)
            throw new InvalidOperationException($"Stadium {stadium._name} not found");

        _stadiums.Remove(existingStadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return _stadiums;
    }
}