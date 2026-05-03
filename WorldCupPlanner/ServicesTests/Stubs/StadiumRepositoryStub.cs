using Domain;
using DomainTests.Stubs;
using IRepository;

namespace ServicesTests.Stubs;

public class StadiumRepositoryStub : IStadiumRepository
{
    private readonly List<IStadium> _stadiums = new();

    public void Add(IStadium stadium)
    {
        _stadiums.Add(stadium);
    }

    public IStadium GetByName(string name)
    {
        var stadium = _stadiums.FirstOrDefault(s => s._stadiumName == name);

        if (stadium == null)
            throw new KeyNotFoundException("Stadium not found");

        return stadium;
    }
}