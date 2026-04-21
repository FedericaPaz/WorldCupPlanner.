using Domain;
using IRepository;

namespace Services;

public class StadiumService
{
    private readonly IStadiumRepository _stadiumRepository;

    public StadiumService(IStadiumRepository stadiumRepository)
    {
        _stadiumRepository = stadiumRepository;
    }

    public void AddStadium(string name, string city, string? description, int locativeCapacity)
    {
        var stadium= new Stadium(name, city, description, locativeCapacity);
        _stadiumRepository.AddStadium(stadium);
    }

}