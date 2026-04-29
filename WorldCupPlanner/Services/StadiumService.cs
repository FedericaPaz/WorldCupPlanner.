using Domain;
using DTOs;
using IRepository;
using IServices;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;

    public StadiumService(IStadiumRepository repository)
    {
        _stadiumRepository = repository;
    }

    public void AddStadium(string stadiumName, string stadiumCity, string? stadiumDescription, int locativeCapacity)
    {
        if(stadiumName is null)
            throw new ArgumentNullException(nameof(stadiumName), "Name is required");
        if(stadiumCity is null)
            throw new ArgumentNullException(nameof(stadiumCity), "City is required");
        
        var existingStadium = _stadiumRepository.GetStadium(stadiumName);
        if (existingStadium != null)
            throw new InvalidOperationException("Stadium already exists");
        
        var newStadium = new Stadium(stadiumName, stadiumCity, stadiumDescription, locativeCapacity);
        _stadiumRepository.AddStadium(newStadium);
    }

    public void DeleteStadium(string name)
    {
        var stadium = _stadiumRepository.GetStadium(name);
        if (stadium == null)
            throw new InvalidOperationException("Stadium not found");

        _stadiumRepository.DeleteStadium(stadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return _stadiumRepository.GetAllStadiums();
    }
}