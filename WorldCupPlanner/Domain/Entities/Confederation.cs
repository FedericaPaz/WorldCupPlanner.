namespace Domain.Entities;

public class Confederation
{
    private static readonly Dictionary<string, int> _confederationLimits = new()
    {
        ["UEFA"] = 16,
        ["CONMEBOL"] = 7,
        ["CONCACAF"] = 7,
        ["CAF"] = 9,
        ["AFC"] = 8,
        ["OFC"] = 1
    };

    private string _name = string.Empty;
    private int _maxTeams;

    public string Name
    {
        get { return _name; }
        private set
        {
            EnsureValidName(value);
            _name = NormalizeName(value);
        }
    }

    public int MaxTeams
    {
        get { return _maxTeams; }
        private set { _maxTeams = value; }
    }

    public Confederation(string name)
    {
        Name = name;
        MaxTeams = GetMaxTeamsForName(Name);
    }

    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        string normalizedName = NormalizeName(name);
        return _confederationLimits.ContainsKey(normalizedName);
    }

    public static int GetMaxTeamsForName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return 0;
        }

        string normalizedName = NormalizeName(name);
        return _confederationLimits.GetValueOrDefault(normalizedName, 0);
    }

    public static List<string> GetAllowedNames()
    {
        List<string> allowedNames = new List<string>();

        foreach (string confederationName in _confederationLimits.Keys)
        {
            allowedNames.Add(confederationName);
        }

        allowedNames.Sort();
        return allowedNames;
    }

    public override bool Equals(object? obj)
    {
        return obj is Confederation otherConfederation && Name == otherConfederation.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode(StringComparison.InvariantCulture);
    }

    private static void EnsureValidName(string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name), "Confederation name cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Confederation name cannot be empty or whitespace.", nameof(name));
        }

        string normalizedName = NormalizeName(name);

        if (!_confederationLimits.ContainsKey(normalizedName))
        {
            throw new ArgumentException($"Confederation name '{name}' is invalid.", nameof(name));
        }
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().ToUpperInvariant();
    }
    
    public bool CanAddTeam(int cantidadActual)
    {
        return cantidadActual < MaxTeams;
    }
}