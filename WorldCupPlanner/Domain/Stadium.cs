namespace Domain;

public class Stadium
{
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;
    private const int MinLocativeCapacity = 20000;
    
    public string Name {get; private set;} 
    public string City {get; private set;}
    public string Description {get; private set;}
    public int LocativeCapacity {get; private set;}

    public Stadium(string name, string city, string description, int locativeCapacity)
    {
        ValidateName(name);
        ValidateCity(city);
        ValidateLocativeCapacity(locativeCapacity);
        ValidateDescription(description);
        Name = name;
        City = city;
        Description = description;
        LocativeCapacity = locativeCapacity;
    }
    
    private void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length > MaxNameLength)
            throw new ArgumentException("Name must be between 1 and 80 characters long.");
    }

    private void ValidateCity(string city)
    {
        if(string.IsNullOrWhiteSpace(city) || city.Length > MaxCityLength)
            throw new ArgumentException("City must be between 1 and 60 characters long.");
    }

    private void ValidateLocativeCapacity(int locativeCapacity)
    {
        if(locativeCapacity < MinLocativeCapacity)
            throw new ArgumentException("Locative capacity must be at least 20000.");
    }

    private void ValidateDescription(string description)
    {
        if(string.IsNullOrWhiteSpace(description) || description.Length > MaxDescriptionLength)
            throw new ArgumentException("Description must be between 1 and 400 characters long.");
    }
    
    
    
    
    
}