using Domain;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private List<Stadium> _stadiums = new List<Stadium>();

    public void AddStadium(Stadium stadium)
    {
        _stadiums.Add(stadium);
    }
}