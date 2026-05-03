using System;
using System.Collections.Generic;

namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public int MaxTeams { get; }
    
    public Confederation(string name)
    {
        var validatedName = ValidateName(name);
        Name = NormalizeName(validatedName);
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
        return name.ToUpperInvariant();
    }
}