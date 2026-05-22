using Domain.Entities;
using IRepository;

namespace ServicesTests.Stubs;

public class StadiumRepositoryStub : IStadiumRepository
{
    private readonly List<Stadium> _stadiums = new();

    public void AddStadium(Stadium stadium)
    {
        _stadiums.Add(stadium);
    }

    public Stadium? GetStadium(string name)
    {
        return _stadiums.FirstOrDefault(s => s.StadiumName == name);
    }

    public void DeleteStadium(Stadium stadium)
    {
        _stadiums.Remove(stadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return new List<Stadium>(_stadiums);
    }
}