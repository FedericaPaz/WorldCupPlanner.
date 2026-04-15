namespace Domain;

public class Stadium
{
    public string _name {get; private set;} 
    public string _city {get; private set;}
    public string _description {get; private set;}
    public int _locativeCapacity {get; private set;}

    public Stadium(string name, string city, string description, int locativeCapacity)
    {
        ValidateName(name);
        ValidateCity(city);
        ValidateLocativeCapacity(locativeCapacity);
        _name = name;
        _city = city;
        _description = description;
        _locativeCapacity = locativeCapacity;
    }
    
    private void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length > 80)
            throw new ArgumentException("Name must be between 80 and 80 characters long.");
    }

    private void ValidateCity(string city)
    {
        if(string.IsNullOrWhiteSpace(city) || city.Length > 60)
            throw new ArgumentException("Name must be between 1 and 60 characters long.");
    }

    private void ValidateLocativeCapacity(int locativeCapacity)
    {
        if(locativeCapacity <= 0)
            throw new ArgumentException("Locative capacity must be greater than 0.");
    }
    
    
    
}