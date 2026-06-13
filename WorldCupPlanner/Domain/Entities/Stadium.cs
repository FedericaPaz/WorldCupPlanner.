using Domain.Interfaces;

namespace Domain.Entities;

public class Stadium : IStadium
{
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;
    private const int MinLocativeCapacity = 20000;

    private string _stadiumName = string.Empty;
    private string _cityName = string.Empty;
    private string _stadiumDescription = string.Empty;
    private int _locativeCapacity;

    public string StadiumName
    {
        get => _stadiumName;
        set
        {
            var clean = value?.Trim() ?? string.Empty;
            EnsureValidName(clean);
            _stadiumName = clean;
        }
    }

    public string CityName
    {
        get => _cityName;
        set
        {
            var clean = value?.Trim() ?? string.Empty;
            EnsureValidCity(clean);
            _cityName = clean;
        }
    }

    public string StadiumDescription
    {
        get => _stadiumDescription;
        set
        {
            var clean = value?.Trim() ?? string.Empty;
            EnsureValidDescription(clean);
            _stadiumDescription = clean;
        }
    }

    public int LocativeCapacity
    {
        get => _locativeCapacity;
        set
        {
            EnsureValidCapacity(value);
            _locativeCapacity = value;
        }
    }

    private Stadium()
    {
        
    }
    
    public Stadium(string name, string cityName, string? stadiumDescription, int locativeCapacity)
    {
        StadiumName = name;
        CityName = cityName;
        StadiumDescription = stadiumDescription;
        LocativeCapacity = locativeCapacity;
    }

    private void EnsureValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Stadium name cannot be empty or whitespace.", nameof(name));
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Stadium name must be at most {MaxNameLength} characters.", nameof(name));
    }

    private void EnsureValidCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name cannot be empty or whitespace.", nameof(city));
        if (city.Length > MaxCityLength)
            throw new ArgumentException($"City name must be at most {MaxCityLength} characters.", nameof(city));
    }

    private void EnsureValidDescription(string description)
    {
        if (description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description must be at most {MaxDescriptionLength} characters.", nameof(description));
    }

    private void EnsureValidCapacity(int capacity)
    {
        if (capacity < MinLocativeCapacity)
            throw new ArgumentException($"Capacity must be at least {MinLocativeCapacity}.", nameof(capacity));
    }
}
