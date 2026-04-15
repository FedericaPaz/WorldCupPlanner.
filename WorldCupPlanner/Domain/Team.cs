namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public string Confederation { get; private set; }
    public int FifaRanking { get; private set; }
    
    public Team(string name, string confederation, int fifaRanking)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(confederation))
            throw new ArgumentException("Team confederation cannot be empty.");
        
        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }
    
}