namespace Domain;

public class Stadium
{
    private const int MAX_NAME_LENGTH = 80;
    private const int MAX_CITY_LENGTH = 60;
    private const int MAX_DESCRIPTION_LENGTH = 400;
    private const int MIN_LOCATIVE_CAPACITY = 20000;
    
    public string _name {get; private set;} 
    public string _city {get; private set;}
    public string _description {get; private set;}
    public int _locativeCapacity {get; private set;}

    public Stadium(string name, string city, string description, int locativeCapacity)
    {
        ValidateName(name);
        ValidateCity(city);
        ValidateLocativeCapacity(locativeCapacity);
        ValidateDescription(description);
        _name = name;
        _city = city;
        _description = description;
        _locativeCapacity = locativeCapacity;
    }
    
    private void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH)
            throw new ArgumentException("Name must be between 1 and 80 characters long.");
    }

    private void ValidateCity(string city)
    {
        if(string.IsNullOrWhiteSpace(city) || city.Length > MAX_CITY_LENGTH)
            throw new ArgumentException("City must be between 1 and 60 characters long.");
    }

    private void ValidateLocativeCapacity(int locativeCapacity)
    {
        if(locativeCapacity < MIN_LOCATIVE_CAPACITY)
            throw new ArgumentException("Locative capacity must be at least 20000.");
    }

    private void ValidateDescription(string description)
    {
        if(string.IsNullOrWhiteSpace(description) || description.Length > MAX_DESCRIPTION_LENGTH)
            throw new ArgumentException("Description must be between 1 and 400 characters long.");
    }
    
    
    
    
    
}