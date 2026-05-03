namespace Domain;

public class Stadium
{
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;
    private const int MinLocativeCapacity = 20000;

    private string _stadiumName;
    private string _cityName;
    private string _stadiumDescription;
    private int _locativeCapacity;

    public string StadiumName
    {
        get => _stadiumName;
        set
        {
            EnsureValidName(value);
            _stadiumName = value.Trim();
        }
    }

    public string CityName
    {
        get => _cityName;
        set
        {
            EnsureValidCity(value);
            _cityName = value.Trim();
        }
    }

    public string StadiumDescription
    {
        get => _stadiumDescription;
        set
        {
            EnsureValidDescription(value);
            _stadiumDescription = value?.Trim();
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

    public Stadium(string name, string cityName, string stadiumDescription, int locativeCapacity)
    {
        StadiumName = name;
        CityName = cityName;
        StadiumDescription = stadiumDescription;
        LocativeCapacity = locativeCapacity;
    }
    
    private void EnsureValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > MaxNameLength)
            throw new ArgumentException($"Stadium name must be between 1 and {MaxNameLength} characters.");
    }

    private void EnsureValidCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city) || city.Length > MaxCityLength)
            throw new ArgumentException($"City name must be between 1 and {MaxCityLength} characters.");
    }

    private void EnsureValidDescription(string description)
    {
        if (description is { Length: > MaxDescriptionLength })
            throw new ArgumentException($"Description must be at most {MaxDescriptionLength} characters.");
    }
    
    private void EnsureValidCapacity(int capacity)
    {
        if (capacity < MinLocativeCapacity)
            throw new ArgumentException($"Capacity must be at least {MinLocativeCapacity}.");
    }
}