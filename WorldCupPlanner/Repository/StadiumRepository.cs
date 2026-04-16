using Domain;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private List<Stadium> _stadiums = new List<Stadium>();

    public void AddStadium(Stadium stadium)
    {
        if(stadium is null)
            throw new ArgumentNullException(nameof(stadium));
        
        if(_stadiums.Any(s => s._name == stadium._name))
            throw new ArgumentException($"Stadium {stadium._name} already exists");
        
        _stadiums.Add(stadium);
    }

    public Stadium? GetStadium(string name)
    {
        return _stadiums.FirstOrDefault(stadium => stadium._name == name);
    }

    public void UpdateStadium(Stadium stadium)
    {
        var existingStadium = _stadiums.FirstOrDefault(s => s._name == stadium._name);
        if(existingStadium is null)
            throw new InvalidOperationException($"Stadium {stadium._name} not found");
        _stadiums.Remove(existingStadium);
        _stadiums.Add(stadium);
    }

    public void DeleteStadium(string name)
    {
        var stadium = _stadiums.FirstOrDefault(s => s._name == name);
        if(stadium is null)
            throw new InvalidOperationException($"Stadium {name} not found");
        _stadiums.Remove(stadium);
    }
    
}