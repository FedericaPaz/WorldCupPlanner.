using System;
using System.Collections.Generic;

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
        var validatedName = ValidateName(name);
        var normalizedName = NormalizeName(validatedName);
        
        if (!_confederationLimits.ContainsKey(normalizedName))
        {
            throw new ArgumentException($"Confederation name '{name}' is invalid.");
        }
        
        Name = normalizedName;
        MaxTeams = _confederationLimits[normalizedName];
    }
    
    public static int GetMaxTeamsForName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 0;
        var normalized = name.Trim().ToUpperInvariant();
        return _confederationLimits.GetValueOrDefault(normalized, 0);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Confederation other && Name == other.Name;
    }
    
    private static string ValidateName(string name)
    {
        if (string.Equals(null,name))
            throw new ArgumentNullException(nameof(name), "Confederation name cannot be null.");
            
        if (string.IsNullOrEmpty(name))
             throw new ArgumentException("Confederation name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Confederation name cannot be empty.");
        
        return name;
    }
    
    private static string NormalizeName(string name)
    {
        return name.Trim().ToUpperInvariant();
    }
    
}