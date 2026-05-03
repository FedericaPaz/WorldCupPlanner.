
namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public int MaxTeams { get; }

    private static readonly Dictionary<string, int> _confederationLimits = new()
    {
        ["UEFA"] = 16,
        ["CONMEBOL"] = 7,
        ["CONCACAF"] = 7,
        ["CAF"] = 9,
        ["AFC"] = 8,
        ["OFC"] = 1
    };

    public Confederation(string name)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name), "Confederation name cannot be null.");

        var normalized = NormalizeName(name);

        if (string.IsNullOrEmpty(normalized))
            throw new ArgumentException("Confederation name cannot be empty or whitespace.", nameof(name));

        if (!_confederationLimits.ContainsKey(normalized))
            throw new ArgumentException($"Confederation name '{name}' is invalid.", nameof(name));

        Name = normalized;
        MaxTeams = _confederationLimits[normalized];
    }
    
    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var normalized = NormalizeName(name);
        return _confederationLimits.ContainsKey(normalized);
    }

    public static int GetMaxTeamsForName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 0;

        var normalized = NormalizeName(name);
        return _confederationLimits.GetValueOrDefault(normalized, 0);
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().ToUpperInvariant();
    }

    public override bool Equals(object? obj)
    {
        return obj is Confederation other && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode(StringComparison.InvariantCulture);
    }
    
    public static List<string> GetAllowedNames()
    {
        return new List<string> 
        { 
            "AFC", "CAF", "CONCACAF", "CONMEBOL", "OFC", "UEFA" 
        };
    }
}