using System;
using System.Collections.Generic;

namespace Domain;

public class Confederation
{
    public string Name { get; private set; }
    public int MaxTeams { get; }
    
    public Confederation(string name)
    {
        Name = name;
    }
}