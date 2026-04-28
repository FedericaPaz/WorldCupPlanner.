using System;

namespace Domain;

public class Team
{
    private const int MaxNameLength = 60;
    private const int MinRanking = 300;
    private const int MaxRanking = 2500;

    private string _name;
    private Confederation _confederation;
    private int _fifaRanking;
    
    public string Name
    {
        get => _name;
        private set => _name = value;
    }

    public Confederation Confederation
    {
        get => _confederation;
        private set => _confederation = value;
    }

    public int FifaRanking
    {
        get => _fifaRanking;
        private set => _fifaRanking = value;
    }
    
    public Team(string name, Confederation confederation, int fifaRanking)
    {
        ValidateName(name);
        ValidateConfederation(confederation);
        ValidateRanking(fifaRanking);

        _name = name;
        _confederation = confederation;
        _fifaRanking = fifaRanking;
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