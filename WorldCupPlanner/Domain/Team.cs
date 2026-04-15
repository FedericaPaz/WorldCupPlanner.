namespace Domain;

public class Team
{
    public string Name { get; private set; }
    public string Confederation { get; private set; }
    public int FifaRanking { get; private set; }
    
    public Team(string name, string confederation, int fifaRanking)
    {
        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }
    
}