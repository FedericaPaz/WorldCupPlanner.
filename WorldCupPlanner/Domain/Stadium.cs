namespace Domain;

public class Stadium
{
    public string _name {get; private set;} 
    public string _city {get; private set;}
    public string _description {get; private set;}
    public int _locativeCapacity {get; private set;}

    public Stadium(string name, string city, string description, int locativeCapacity)
    {
        if(string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be empty.");
        _name = name;
        _city = city;
        _description = description;
        _locativeCapacity = locativeCapacity;
    }
}