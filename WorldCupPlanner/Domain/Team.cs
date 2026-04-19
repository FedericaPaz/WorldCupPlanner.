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
        ValidateName(name);
        ValidateConfederation(confederation);
        ValidateRanking(fifaRanking);

        Name = name;
        Confederation = confederation;
        FifaRanking = fifaRanking;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Name cannot exceed {MaxNameLength} characters");
    }

    private void ValidateConfederation(Confederation confederation)
    {
        if (confederation is null)
            throw new ArgumentNullException(nameof(confederation));
    }

    private void ValidateRanking(int ranking)
    {
        if (ranking < MinRanking || ranking > MaxRanking)
            throw new ArgumentException($"Ranking must be between {MinRanking} and {MaxRanking}");
    }
}