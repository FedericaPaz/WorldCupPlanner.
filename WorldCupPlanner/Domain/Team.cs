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
        
        if (fifaRanking < 300)
            throw new ArgumentException("Fifa ranking cannot be less than 300.");
        
        if (fifaRanking > 2500)
            throw new ArgumentException("Fifa ranking cannot be more than 2500.");
        
        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }
    
}