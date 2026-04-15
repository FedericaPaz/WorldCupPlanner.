namespace Domain;

public class Confederation
{
    private static readonly List<string> ValidNames = new List<string>
    {
        "AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA"
    };
    
    public string Name { get; private set; }

    public Confederation(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Mandatory confederation name.");
        }

        if (!ValidNames.Contains(name))
        {
            throw new ArgumentException("Invalid confederation name.");
        }
        Name = name;
    }
}