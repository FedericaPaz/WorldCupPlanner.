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
        bool nameAlreadyExists = _stadiumRepository
            .GetAllStadiums()
            .Any(s => s._name == name);
        if (nameAlreadyExists)
            throw new ArgumentException($"Stadium with name {name} already exists");
        
        var stadium= new Stadium(name, city, description, locativeCapacity);
        _stadiumRepository.AddStadium(stadium);
    }

    public void DeleteStadium(Stadium stadium)
    {
        _stadiumRepository.DeleteStadium(stadium);
    }

    public void UpdateStadium(Stadium stadium)
    {
        bool stadiumAlreadyExists = _stadiumRepository
            .GetAllStadiums()
            .Any(s => s._name == stadium._name);
        if (stadiumAlreadyExists)
            throw new ArgumentException($"Stadium with name {stadium._name} already exists");
        _stadiumRepository.UpdateStadium(stadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return _stadiumRepository.GetAllStadiums();
    }

    public Stadium? GetStadium(Stadium stadium)
    {
        return _stadiumRepository.GetStadium(stadium);
    }
    

}