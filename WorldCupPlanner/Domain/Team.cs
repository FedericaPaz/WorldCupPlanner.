namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public Confederation Confederation { get; private set; }
    public int FifaRanking { get; private set; }
    
    public Team(string name, Confederation confederation, int fifaRanking)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be empty.");
        
        if (name.Length > 60)
            throw new ArgumentException("Team name cannot be longer than 60 characters.");
        
        if (confederation == null)
            throw new ArgumentException("Confederation cannot be null.");
        
        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }
    
}