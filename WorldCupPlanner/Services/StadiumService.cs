using Domain;
using IRepository;

namespace Services;

public class StadiumService
{
    private readonly IStadiumRepository _stadiumRepository;

    public StadiumService(IStadiumRepository repository)
    {
        _stadiumRepository = repository;
    }

    public void CreateStadium(string stadiumName, string stadiumCity, string? stadiumDescription, int locativeCapacity)
    {
        var newStadium = new Stadium(stadiumName, stadiumCity, stadiumDescription, locativeCapacity);
        _stadiumRepository.AddStadium(newStadium);
    }


}