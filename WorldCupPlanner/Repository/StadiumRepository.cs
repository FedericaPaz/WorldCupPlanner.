using Domain;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private readonly List<Stadium> _stadiums = new List<Stadium>();

    public void AddStadium(Stadium stadium)
    {
        if (stadium == null)
            throw new ArgumentNullException(nameof(stadium));   
            
        _stadiums.Add(stadium);
    }

    public Stadium? GetStadium(string nameStadium)
    {
        if (string.IsNullOrWhiteSpace(nameStadium))
            return null;
        return _stadiums.FirstOrDefault(s => s.StadiumName.Equals(nameStadium, StringComparison.OrdinalIgnoreCase));
    }
    
    public void DeleteStadium(Stadium stadium)
    {
        _stadiums.Remove(stadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return _stadiums.ToList();
    }
}