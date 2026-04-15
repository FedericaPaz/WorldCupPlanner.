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
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
    }

    private void ValidateCity(string city)
    {
        if(string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.");
    }

    public void ValidateLocativeCapacity(int locativeCapacity)
    {
        if(locativeCapacity <= 0)
            throw new ArgumentException("Locative capacity must be greater than 0.");
    }
    
    
}