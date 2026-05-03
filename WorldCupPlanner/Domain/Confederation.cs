using System;
using System.Collections.Generic;

namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public int MaxTeams { get; }
    
    public Confederation(string name)
    {
        if(string.Equals(null,name))
            throw new ArgumentNullException(nameof(name), "Confederation name cannot be null.");
            
        if (string.IsNullOrEmpty(name))
             throw new ArgumentException("Confederation name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Confederation name cannot be empty.");
        
        Name = name;
    }
    
    
}