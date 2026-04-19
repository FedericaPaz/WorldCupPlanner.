namespace Domain;

public class Team
{
    private const int MaxNameLength = 60;
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;
    
    public string Name { get; private set; }
    public Confederation Confederation { get; private set; }
    public int FifaRanking { get; private set; }
    
    public Team(string name, Confederation confederation, int fifaRanking)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be empty.");
        
        if (name.Length > MaxNameLength)
            throw new ArgumentException("Team name cannot be longer than 60 characters.");
        
        if (confederation is null)
            throw new ArgumentNullException("Confederation cannot be null.");
        
        if (fifaRanking < MinRanking)
            throw new ArgumentException("Fifa ranking cannot be less than 300.");
        
        if (fifaRanking > MaxRanking)
            throw new ArgumentException("Fifa ranking cannot be more than 2500.");
        
        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }
    
}