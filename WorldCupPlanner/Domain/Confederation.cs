using System;
using System.Collections.Generic;

namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public int MaxTeams { get; }
    
    public Confederation(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Confederation name cannot be empty or whitespace.");
        
        Name = name;
    }
    
    
}