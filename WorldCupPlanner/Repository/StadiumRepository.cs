using Domain;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private readonly List<Stadium> _stadiums = new List<Stadium>();

    public void AddStadium(Stadium? stadium)
    {
        _stadiums.Add(stadium);
    }

    public Stadium? GetStadium(string nameStadium)
    {
        return _stadiums.FirstOrDefault(s => s._name == nameStadium);
    }
    
    public void DeleteStadium(Stadium? stadium)
    {
        _stadiums.Remove(stadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return _stadiums;
    }
}